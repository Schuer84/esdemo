using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Persistence;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Queries.Account
{
    public class GetAccountTransactionsQueryHandler : QueryHandler<AccountDbContext, Transaction, IEnumerable<Transaction>>
    {
        public GetAccountTransactionsQueryHandler(AccountDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Transaction>> Query(IQuery<Transaction> query, CancellationToken cancellationToken)
        {
            return await PrepareQuery(query).ToListAsync(cancellationToken);
        }
    }
}