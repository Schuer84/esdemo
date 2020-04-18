using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public class AmountDeposited : TransactionEvent
    {
        public AmountDeposited() 
            : base(MessageTypes.Transaction.Deposited)
        {
        }
    }
}