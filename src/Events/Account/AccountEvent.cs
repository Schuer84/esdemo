using System;

namespace SqlStreamStore.Demo.Events.Account
{
    public abstract class AccountEvent : Event
    {
        public Guid TransactionId { get; }
        public decimal Amount { get; }
        public DateTime DateTime { get; }
        
        public AccountEvent(Guid transactionId, decimal amount, DateTime dateTime, string type) 
            : base(type)
        {
            TransactionId = transactionId;
            Amount = amount;
            DateTime = dateTime;
        }
    }
}