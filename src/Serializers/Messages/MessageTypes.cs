namespace SqlStreamStore.Demo
{
    public class MessageTypes
    {
        public class Account
        {
            public class Balance
            {
                public const string Withdrawn = nameof(Withdrawn);
                public const string Deposited = nameof(Deposited);
            }
        }
    }
}