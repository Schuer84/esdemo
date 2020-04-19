using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;
using SqlStreamStore.Demo.Commands;
using SqlStreamStore.Demo.Commands.Account;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Demo.Persistence;
using SqlStreamStore.Demo.Persistence.Entities;
using SqlStreamStore.Demo.Queries;
using SqlStreamStore.Demo.Queries.Account;
using SqlStreamStore.Demo.Serializers.Json;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Demo.Serializers.Messages.Account;
using SqlStreamStore.Demo.Services;

namespace SqlStreamStore.Demo
{
    public class ServiceCollectionConfiguration : IServiceCollectionConfiguration
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MsSqlStreamStoreSettings>(x => new MsSqlStreamStoreSettings("Server=localhost;Database=accounts-db;Trusted_Connection=True;"));
            serviceCollection.AddSingleton<IStreamStore, MsSqlStreamStore>();
            serviceCollection.AddSingleton<MsSqlStreamStore>();
            serviceCollection.AddSingleton<IEventStore, EventStore>();
            serviceCollection.AddSingleton<AggregateRepository>();
            serviceCollection.AddTransient<AccountAggregate>();
            serviceCollection.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
            serviceCollection.AddSingleton<IMessageSerializer, DepositMessageSerializer>();
            serviceCollection.AddSingleton<IMessageSerializer, WithdrawnMessageSerializer>();
            serviceCollection.AddTransient<IEventSerializer, EventSerializer>();

            serviceCollection.AddTransient<IAggregateRepository, AggregateRepository>();

            serviceCollection.AddTransient<ICommandHandler, CommandHandler>();
            serviceCollection.AddTransient<ICommandHandler<WithdrawAmountCommand>, WithdrawAmountCommandHandler>();
            serviceCollection.AddTransient<ICommandHandler<DepositAmountCommand>, DepositAmountCommandHandler>();

            serviceCollection.AddTransient<IAccountService, AccountService>();

            serviceCollection.AddTransient<AccountDbContext>(x => new AccountDbContext("Server=localhost;Database=accounts-db;Trusted_Connection=True;"));

            serviceCollection.AddTransient<IQueryHandler, QueryHandler>();
            serviceCollection.AddTransient<IQueryHandler<IQuery<Transaction>, decimal>, GetAccountBalanceQueryHandler>();
            serviceCollection.AddTransient<IQueryHandler<IQuery<Transaction>, IEnumerable<Transaction>>, GetAccountTransactionsQueryHandler>();

        }
    }
}