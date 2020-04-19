using System;

namespace SqlStreamStore.Demo.Persistence.Entities
{
    public class Transaction : Entity
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
