using SqlStreamStore.Demo.Events.Account;
using SqlStreamStore.Demo.Serializers.Json;

namespace SqlStreamStore.Demo.Serializers.Messages.Account
{
    public class DepositMessageSerializer : JsonMessageSerializer<AmountDeposited>
    {
        public DepositMessageSerializer(IJsonSerializer jsonSerializer) 
            : base(MessageTypes.Transaction.Deposited, jsonSerializer)
        {
        }
    }
}