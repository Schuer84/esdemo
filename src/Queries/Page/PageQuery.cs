using System;
using System.Linq;

namespace SqlStreamStore.Demo.Queries.Page
{
    public abstract class PageQuery<TEntity> : IPageQuery<TEntity>
    {
        public abstract IQueryable<TEntity> GetQuery(IQueryable<TEntity> source);
        

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
       
        IQueryable IQuery.GetQuery(IQueryable source)
        {
            var query = source as IQueryable<TEntity>;
            if (query == null)
            {
                throw new InvalidOperationException("Invalid queryable provided");
            }
            return GetQuery(query);
        }
    }
}