using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading;
using LiquidProjections;
using LiquidProjections.EntityFramework;
using SqlStreamStore.Demo.Events.Account;
using SqlStreamStore.Demo.Persistence.Entities;
using Transaction = SqlStreamStore.Demo.Persistence.Entities.Transaction;

namespace SqlStreamStore.Demo.Projectors
{
    public class AccountProjector
    {
        private readonly Dispatcher _dispatcher;
        private readonly EntityFrameworkProjector<Transaction, Guid, ProjectorState> _transactionProjector;
        private readonly EntityFrameworkProjector<Account, Guid, ProjectorState> _accountProjector;

        public AccountProjector(Func<DbContext> dbContextProvider, Dispatcher dispatcher)
        {
            if (dbContextProvider == null) throw new ArgumentNullException(nameof(dbContextProvider));
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            _dispatcher = dispatcher;
            _transactionProjector = BuildTransactionProjector(dbContextProvider);
        }

        public void Start()
        {
            long? lastCheckpoint = _transactionProjector.GetLastCheckpoint();

            _dispatcher.Subscribe(lastCheckpoint, async (transactions, info) =>
            {
                await _transactionProjector.Handle(transactions, info.CancellationToken ?? CancellationToken.None);
            });
        }

        private EntityFrameworkProjector<Transaction, Guid, ProjectorState> BuildTransactionProjector(Func<DbContext> dbContextProvider)
        {
            var transactionMapBuilder = new EventMapBuilder<Transaction, Guid, EntityFrameworkProjectionContext>();
                transactionMapBuilder
                    .Map<AmountDeposited>()
                    .AsCreateOf(@event => @event.TransactionId)
                    .Using((entity, @event) =>
                    {
                        entity.Type = TransactionType.Deposit;
                        entity.Amount = @event.Amount;
                        entity.AccountId = @event.AccountId;
                        entity.CreatedOn = DateTime.UtcNow;
                    });

            return new EntityFrameworkProjector<Transaction, Guid, ProjectorState>(dbContextProvider, transactionMapBuilder, (entity, key) => entity.Id = key); 
        }

        //private EntityFrameworkProjector<Account, Guid, ProjectorState> BuildAccountProjector()
        //{
        //    var transactionMapBuilder = new EventMapBuilder<Account, Guid, EntityFrameworkProjectionContext>();
        //    transactionMapBuilder
        //        .Map<AmountDeposited>()
        //        .AsCreateOf(@event => @event.TransactionId)
        //        .Using((entity, @event) =>
        //        {
        //            entity.Type = TransactionType.Deposit;
        //            entity.Amount = @event.Amount;
        //            entity.AccountId = @event.AccountId;
        //            entity.CreatedOn = DateTime.UtcNow;
        //        });

        //    return new EntityFrameworkProjector<Account, Guid, ProjectorState>(_dbContextProvider, transactionMapBuilder, (entity, key) => entity.Id = key);
        //}
    }
}
