using System;
using System.Collections.Generic;

namespace SqlStreamStore.Demo.Persistence.Entities
{
    public class Account : Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
