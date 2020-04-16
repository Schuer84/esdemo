using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public class AmountDeposited : AccountEvent
    {
        public AmountDeposited(Guid transactionId, decimal amount, DateTime dateTime) 
            : base(transactionId, amount, dateTime, MessageTypes.Account.Balance.Deposited)
        {
        }
    }
}