using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo.Aggregates
{
    public class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IServiceProvider _serviceProvider;

        public AggregateRepository(IEventStore eventStore, IServiceProvider serviceProvider)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TAggregate> GetById<TAggregate>(StreamId id, CancellationToken cancellationToken)
            where TAggregate: IEventSourcedAggregate
        {
            var aggregate = _serviceProvider.GetRequiredService<TAggregate>();
                aggregate.Init(id);

            var events = await _eventStore.GetAllAsync(id, cancellationToken);

            foreach (var @event in events)
            {
                //todo: overload with params/ienumerable?
                aggregate.Apply(@event.ExpectedVersion, @event);
            }
            return aggregate;
        }

        public async Task Save<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken)
            where TAggregate: IEventSourcedAggregate
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
            
            var changeset = aggregate.GetChangeSet();
            foreach (var @event in changeset.Events)
            {
                await _eventStore.AppendAsync(aggregate.Id, @event, cancellationToken);
            }
        }
    }
}