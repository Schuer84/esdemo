using System;

namespace SqlStreamStore.Demo.Events.Account
{
    public abstract class TransactionEvent : Event
    {
        public Guid AccountId { get; set; }
        public Guid TransactionId { get; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public DateTime DateTime { get; } = DateTime.UtcNow;

        protected TransactionEvent(string type) 
            : base(type)
        { }
    }
}