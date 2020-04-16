using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public class AmountWithdrawn : AccountEvent
    {
        public AmountWithdrawn(Guid transactionId, decimal amount, DateTime dateTime) 
            : base(transactionId, amount, dateTime, MessageTypes.Account.Balance.Withdrawn)
        {
        }
    }
}