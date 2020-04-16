using System;

namespace SqlStreamStore.Demo
{
    public class Withdrawn : AccountEvent
    {
        public Withdrawn(Guid transactionId, decimal amount, DateTime dateTime) 
            : base(transactionId, amount, dateTime, MessageTypes.Account.Balance.Withdrawn)
        {
        }
    }
}