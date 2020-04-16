using System.Linq;

namespace SqlStreamStore.Demo.Queries
{
    public interface IQuery
    {
        IQueryable GetQuery(IQueryable source);
    }

    public interface IQuery<TEntity> : IQuery
    {
        IQueryable<TEntity> GetQuery(IQueryable<TEntity> source);
    }
}
