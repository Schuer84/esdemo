using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiquidProjections.EntityFramework
{
    public sealed class EntityFrameworkProjector<TProjection, TKey, TState>
        where TProjection : class, new()
        where TState : class, IProjectorState, new()
    {
        private readonly Func<DbContext> _dbContextFactory;
        private readonly EntityFrameworkEventMapConfigurator<TProjection, TKey> _mapConfigurator;
        private int _batchSize = 1;
        private string _stateKey = typeof(TProjection).Name;
        private HandleException _exceptionHandler = (exception, _, __) => Task.FromResult(ExceptionResolution.Abort);

        public EntityFrameworkProjector(
            Func<DbContext> dbContextFactory,
            IEventMapBuilder<TProjection, TKey, EntityFrameworkProjectionContext> mapBuilder,
            Action<TProjection, TKey> setIdentity)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapConfigurator = new EntityFrameworkEventMapConfigurator<TProjection, TKey>(mapBuilder, setIdentity);
        }

        /// <summary>
        /// How many transactions should be processed together in one database transaction. Defaults to one.
        /// </summary>
        public int BatchSize
        {
            get => _batchSize;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                _batchSize = value;
            }
        }


        /// <summary>
        /// The key to store the projector state as <typeparamref name="TState"/>.
        /// </summary>
        public string StateKey
        {
            get => _stateKey;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("State key is missing.", nameof(value));
                _stateKey = value;
            }
        }
        /// <summary>
        /// Sets the behavior for when the state of the projector is persisted to the database. 
        /// </summary>
        public PersistStateBehavior PersistStateBehavior { get; set; } = PersistStateBehavior.EveryBatch;

        /// <summary>
        /// Allows enriching the projector state with additional details before the updated state is written to the database.
        /// </summary>
        /// <remarks>
        /// Is called within the scope of the NHibernate transaction that is created by <see cref="Handle"/>.
        /// </remarks>
        public EnrichState<TState> EnrichState { get; set; } = (state, transaction) => { };

        /// <summary>
        /// A delegate that will be executed when projecting a batch of transactions fails
        /// and which allows the consuming code to decide how to handle the exception. 
        /// </summary>
        public HandleException ExceptionHandler
        {
            get => _exceptionHandler;
            set => _exceptionHandler = value ?? throw new ArgumentNullException(nameof(value), "Retry policy is missing.");
        }

        /// <summary>
        /// Defines a filter that can be used to skip certain projections from being updated.
        /// </summary>
        public Predicate<TProjection> Filter
        {
            get => _mapConfigurator.Filter;
            set => _mapConfigurator.Filter = value ?? throw new ArgumentNullException(nameof(value), "A filter cannot be null");
        }

        /// <summary>
        /// Instructs the projector to project a collection of ordered <paramref name="transactions"/> asynchronously
        /// in batches of the configured size <see cref="BatchSize"/>. Should cancel its work
        /// when the <paramref name="cancellationToken"/> is triggered.
        /// </summary>
        public async Task Handle(IReadOnlyList<Transaction> transactions, CancellationToken cancellationToken)
        {
            if (transactions == null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }

            long? lastCheckpoint = GetLastCheckpoint();
            IEnumerable<Batch<Transaction>> transactionBatches = transactions
                .Where(t => (!lastCheckpoint.HasValue) || (t.Checkpoint > lastCheckpoint))
                .InBatchesOf(BatchSize);

            foreach (Batch<Transaction> batch in transactionBatches)
            {
                await ProjectUnderPolicy(batch.ToList(), batch.IsLast, cancellationToken).ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }


        private async Task ProjectUnderPolicy(IList<Transaction> batch, bool isLastBatchOfPage, CancellationToken cancellationToken, int attempts = 0)
        {
            bool individualRetry = (attempts > 0);
            bool retry = false;
            do
            {
                try
                {
                    attempts++;
                    await ProjectTransactionBatch(batch, isLastBatchOfPage || retry, cancellationToken).ConfigureAwait(false);
                    retry = false;
                }
                catch (ProjectionException exception)
                {
                    ExceptionResolution resolution = await ExceptionHandler(exception, attempts, cancellationToken).ConfigureAwait(false);
                    switch (resolution)
                    {
                        case ExceptionResolution.Abort:
                            throw;

                        case ExceptionResolution.Retry:
                            retry = true;
                            break;

                        //case ExceptionResolution.RetryIndividual:
                        //    if (individualRetry)
                        //    {
                        //        throw new InvalidOperationException("You're already retrying individual transactions");
                        //    }

                        //    foreach (Transaction transaction in batch)
                        //    {
                        //        await ProjectUnderPolicy(new[] { transaction }, true, cancellationToken, attempts);
                        //    }

                        //    break;

                        case ExceptionResolution.Ignore:
                            break;
                    }
                }
            }
            while (retry);
        }

        private async Task ProjectTransactionBatch(IList<Transaction> batch, bool isLastBatchOfPage, CancellationToken cancellationToken)
        {
            try
            {
                using (var dbContext = _dbContextFactory())
                using (var tx = dbContext.Database.BeginTransaction())
                {
                    bool dirty = false;
                    foreach (Transaction transaction in batch)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        dirty |= await ProjectTransaction(transaction, dbContext).ConfigureAwait(false);
                    }

                    if (isLastBatchOfPage
                        || PersistStateBehavior == PersistStateBehavior.EveryBatch
                        || (dirty && PersistStateBehavior == PersistStateBehavior.DirtyBatch))
                    {
                        StoreLastCheckpoint(dbContext, batch.Last());
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    tx.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                //Cache.Clear();
            }
            catch (ProjectionException projectionException)
            {
                //Cache.Clear();

                projectionException.Projector = typeof(TProjection).ToString();
                projectionException.SetTransactionBatch(batch);
                throw;
            }
            catch (Exception exception)
            {
                //Cache.Clear();

                var projectionException = new ProjectionException("Projector failed to project transaction batch.", exception)
                {
                    Projector = typeof(TProjection).ToString()
                };

                projectionException.SetTransactionBatch(batch);
                throw projectionException;
            }
        }

        private async Task<bool> ProjectTransaction(Transaction transaction, DbContext session)
        {
            bool dirty = false;
            foreach (EventEnvelope eventEnvelope in transaction.Events)
            {
                var context = new EntityFrameworkProjectionContext(session)
                {
                    TransactionId = transaction.Id,
                    StreamId = transaction.StreamId,
                    TimeStampUtc = transaction.TimeStampUtc,
                    Checkpoint = transaction.Checkpoint,
                    EventHeaders = eventEnvelope.Headers,
                    TransactionHeaders = transaction.Headers
                };

                try
                {
                    await _mapConfigurator.ProjectEvent(eventEnvelope.Body, context).ConfigureAwait(false);
                    dirty |= context.WasHandled;
                }
                catch (ProjectionException projectionException)
                {
                    projectionException.TransactionId = transaction.Id;
                    projectionException.CurrentEvent = eventEnvelope;
                    throw;
                }
                catch (Exception exception)
                {
                    throw new ProjectionException("Projector failed to project an event.", exception)
                    {
                        TransactionId = transaction.Id,
                        CurrentEvent = eventEnvelope
                    };
                }
            }

            return dirty;
        }

        private void StoreLastCheckpoint(DbContext session, Transaction transaction)
        {
            try
            {
                DbSet<TState> stateSet = session.Set<TState>();
                TState existingState = stateSet.Find(StateKey);
                TState state = existingState ?? new TState { Id = StateKey };
                state.Checkpoint = transaction.Checkpoint;
                state.LastUpdateUtc = DateTime.UtcNow;

                if (existingState == null)
                {
                    stateSet.Add(state);
                }

                EnrichState(state, transaction);
            }
            catch (Exception exception)
            {
                throw new ProjectionException("Projector failed to store last checkpoint.", exception);
            }
        }

        /// <summary>
        /// Determines the checkpoint of the last projected transaction.
        /// </summary>
        public long? GetLastCheckpoint()
        {
            using var dbContext = _dbContextFactory();
            return dbContext.Set<TState>()?.Find(StateKey)?.Checkpoint;
        }
    }

    /// <summary>
    /// A delegate that can be implemented to retry projecting a batch of transactions when it fails.
    /// </summary>
    /// <returns>Returns true if the projector should retry to project the batch of transactions, false if it shoud fail with the specified exception.</returns>
    /// <param name="exception">
    /// The exception that occured that caused this batch to fail. Notice that the batch of exceptions is exposed through
    /// <see cref="ProjectionException.TransactionBatch"/>.
    /// </param>
    /// <param name="attempts">
    /// Number of attempts that were made to project this batch of transactions (includes the one that raised the exception).
    /// </param>
    /// <param name="cancellationToken">
    /// Is requested when the consuming system has canceled the subscription. 
    /// </param>
    public delegate Task<ExceptionResolution> HandleException(ProjectionException exception, int attempts, CancellationToken cancellationToken);

    /// <summary>
    /// Defines the signature of a method that can be used to update the projection state as explained 
    /// in <see cref="EntityFrameworkProjector{TProjection,TKey,TState}.EnrichState"/>.
    /// </summary>
    public delegate void EnrichState<in TState>(TState state, Transaction transaction)
        where TState : IProjectorState;
}
