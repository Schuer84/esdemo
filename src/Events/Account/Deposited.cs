using System;

namespace SqlStreamStore.Demo
{
    public class Deposited : AccountEvent
    {
        public Deposited(Guid transactionId, decimal amount, DateTime dateTime) 
            : base(transactionId, amount, dateTime, MessageTypes.Account.Balance.Deposited)
        {
        }
    }
}