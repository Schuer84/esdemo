using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo.Aggregates
{
    public interface IAggregateRepository
    {
        Task<TAggregate> GetById<TAggregate>(StreamId id, CancellationToken cancellationToken)
            where TAggregate: IEventSourcedAggregate;

        Task Save<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken)
            where TAggregate : IEventSourcedAggregate;
    }
}