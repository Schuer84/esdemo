namespace SqlStreamStore.Demo
{
    public class DepositMessageSerializer : JsonMessageSerializer<Deposited>
    {
        public DepositMessageSerializer(IJsonSerializer jsonSerializer) 
            : base(MessageTypes.Account.Balance.Deposited, jsonSerializer)
        {
        }
    }
}