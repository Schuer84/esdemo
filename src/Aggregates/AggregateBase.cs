using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStore.Demo
{
    public abstract class AggregateBase<TState> : IAggregate, IEventSourcedAggregate
        where TState: class, IAggregateState, new()
    {
        private readonly IEventStore _eventStore;
        protected AggregateBase(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TState State { get; protected set; }

        public string Id { get; protected set; }
        public int Version { get; set; }


        void IEventSourcedAggregate.Init(string id)
        {
            Id = id;
            State = new TState();
        }

        void IEventSourcedAggregate.Apply(int version, object payload)
        {
            State.CallNonPublicIfExists("On", payload);
            Version = version;
        }

        public async Task EmitAsync(Event @event, CancellationToken cancellationToken)
        {
            await _eventStore.AppendAsync(Id, @event, cancellationToken);
        }
    }
}