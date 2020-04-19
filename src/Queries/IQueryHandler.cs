using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStore.Demo.Queries
{

    public interface IQueryHandler
    {
        Task<TResult> Query<TEntity, TResult>(IQuery<TEntity> query, CancellationToken cancellationToken);
    }


    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult> Query(TQuery query, CancellationToken cancellationToken);
    }
}