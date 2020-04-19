using SqlStreamStore.Demo.Events.Account;
using SqlStreamStore.Demo.Serializers.Json;

namespace SqlStreamStore.Demo.Serializers.Messages.Account
{
    public class WithdrawnMessageSerializer : JsonMessageSerializer<AmountWithdrawn>
    {   
        public WithdrawnMessageSerializer(IJsonSerializer serializer) 
            : base(MessageTypes.Transaction.Withdrawn, serializer)
        {
        }
    }
}