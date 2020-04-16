namespace SqlStreamStore.Demo.Aggregates
{
    public interface IEventSourcedAggregate : IAggregate
    {
        void Init(string id);
        void Apply(int version, object payload);

        void Apply(IChangeSet changeSet);
        IChangeSet GetChangeSet();
    }
}