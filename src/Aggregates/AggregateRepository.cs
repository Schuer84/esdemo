using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SqlStreamStore.Demo
{
    public class AggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IServiceProvider _serviceProvider;

        public AggregateRepository(IEventStore eventStore, IServiceProvider serviceProvider)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TAggregate> GetById<TAggregate>(string id, CancellationToken cancellationToken)
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
    }
}