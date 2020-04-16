using System.Text;

namespace SqlStreamStore.Demo
{
    public interface IAggregate
    {
        string Id { get; }
        int Version { get; }
    }
}
