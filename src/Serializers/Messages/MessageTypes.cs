namespace SqlStreamStore.Demo.Serializers.Messages
{
    public class MessageTypes
    {
        public class Account
        {
            public static string Registered = $"{nameof(Account)}.{nameof(Registered)}";
        }

        public class Transaction
        {
            public static string Withdrawn = $"{nameof(Transaction)}.{nameof(Withdrawn)}"; 
            public static string Deposited = $"{nameof(Transaction)}.{nameof(Deposited)}"; 
            public static string Transfered = $"{nameof(Transaction)}.{nameof(Transfered)}";
        }
    }
}