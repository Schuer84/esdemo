namespace SqlStreamStore.Demo
{
    public class WithdrawnMessageSerializer : JsonMessageSerializer<Withdrawn>
    {   
        public WithdrawnMessageSerializer(IJsonSerializer serializer) 
            : base(MessageTypes.Account.Balance.Withdrawn, serializer)
        {
        }
    }
}