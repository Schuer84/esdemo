using System;
using System.Linq;
using System.Text;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Queries.Account
{
    public class GetAccountTransactionsQuery : Query<Transaction>
    {
        public Guid AccountId { get; set; }

        public override IQueryable<Transaction> GetQuery(IQueryable<Transaction> source)
        {
            return source.Where(x => x.AccountId == AccountId);
        }
    }
}
