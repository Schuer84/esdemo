using System;

namespace SqlStreamStore.Demo
{
    public class Balance
    {
        public Balance(decimal amount, DateTime asOf)
        {
            Amount = amount;
            AsOf = asOf;
        }
        
        public decimal Amount { get; private set; }
        public DateTime AsOf { get;private set; }

        public void Add(decimal value)
        {
            AsOf = DateTime.UtcNow;
            Amount += value;
        }

        public void Subtract(decimal value)
        {
            AsOf = DateTime.UtcNow;
            Amount -= value;

        }
    }
}