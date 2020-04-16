using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStore.Demo
{
    public interface IEventSourcedAggregate : IAggregate
    {
        void Init(string id);
        void Apply(int version, object payload);

        Task EmitAsync(Event @event, CancellationToken cancellationToken);
    }
}