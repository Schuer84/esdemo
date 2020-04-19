using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SqlStreamStore.Demo.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<TResult> Query<TEntity, TResult>(IQuery<TEntity> query, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetService<IQueryHandler<IQuery<TEntity>, TResult>>();
            if (handler == null)
            {
                throw new InvalidOperationException("no query handler found");
            }

            return handler.Query(query, cancellationToken);
        }
    }
}