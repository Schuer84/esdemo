using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Persistence;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Queries.Account
{
    public class GetAccountBalanceQueryHandler : QueryHandler<AccountDbContext, Transaction, decimal>
    {
        public GetAccountBalanceQueryHandler(AccountDbContext context) : base(context)
        {
        }

        public override async Task<decimal> Query(IQuery<Transaction> query, CancellationToken cancellationToken)
        {

            return await (from transaction in PrepareQuery(query)
                let amount = transaction.Type == TransactionType.Withdrawal 
                    ? -(transaction.Amount)
                    : transaction.Amount
                select amount).SumAsync(cancellationToken);
        }
    }
}