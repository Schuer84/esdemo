namespace SqlStreamStore.Demo.Persistence.Configurations
{
    public class AccountConfiguration : EntityConfiguration<Entities.Account>
    {
        public AccountConfiguration()
        {
            HasRequired(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            HasMany(x => x.Transactions)
                .WithOptional()
                .HasForeignKey(x => x.AccountId);
        }
    }
}