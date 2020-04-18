using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Persistence.Configurations
{
    public class TransactionConfiguration : EntityConfiguration<Transaction>
    {
        public TransactionConfiguration()
        {
            HasRequired(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId);
        }
    }
}