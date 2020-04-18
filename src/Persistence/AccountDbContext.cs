using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Text;
using SqlStreamStore.Demo.Persistence.Configurations;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Persistence
{
    public class AccountDbContext : DbContext
    {

        public AccountDbContext() : base(nameof(AccountDbContext)) { }
        public AccountDbContext(string connectionString) : base(connectionString) { }


        public DbSet<ProjectorState> ProjectorStates { get; set; }


        public DbSet<Entities.Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions{ get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new AccountConfiguration());
            modelBuilder.Configurations.Add(new TransactionConfiguration());

            modelBuilder.Configurations.Add(new ProjectorStateConfiguration());
        }

    }
}
