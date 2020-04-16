namespace SqlStreamStore.Demo.Aggregates
{
    public interface IAggregate
    {
        string Id { get; }
        int Version { get; }
    }
}
