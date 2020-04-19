using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Queries.Account
{
    public abstract class QueryHandler<TContext, TEntity, TResult> : IQueryHandler<IQuery<TEntity>, TResult>
        where TContext: DbContext
        where TEntity: Entity
    {
        private readonly TContext _context;

        protected QueryHandler(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public abstract Task<TResult> Query(IQuery<TEntity> query, CancellationToken cancellationToken);

        protected virtual DbSet<TEntity> GetSet()
        {
            return _context.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> PrepareQuery(IQuery<TEntity> qry)
        {
            var set = GetSet();
            return qry.GetQuery(set);
        }
    }
}