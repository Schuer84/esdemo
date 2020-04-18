using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public class AmountWithdrawn : TransactionEvent
    {
        public AmountWithdrawn() 
            : base(MessageTypes.Transaction.Withdrawn)
        {
        }
    }
}