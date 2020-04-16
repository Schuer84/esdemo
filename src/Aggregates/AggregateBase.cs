using System.Collections.Generic;
using SqlStreamStore.Demo.Events;

namespace SqlStreamStore.Demo.Aggregates
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

        private List<object> PendingChanges { get; set; } = new List<object>();

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

        void IEventSourcedAggregate.Apply(IChangeSet changeSet)
        {
            foreach (var @event in changeSet.Events)
            {
                State.CallNonPublicIfExists("On", @event);
            }

            Version = changeSet.Version;
        }

        IChangeSet IEventSourcedAggregate.GetChangeSet()
        {
            return new ChangeSet()
            {
                Events = PendingChanges.ToArray(),
                Version = Version
            };
        }


        protected void Emit(Event @event)
        {
            State.CallNonPublicIfExists("On", @event);
            Track(@event);
        }

        protected virtual void Track(object @event)
        {
            this.PendingChanges.Add(@event);
        }
    }
}