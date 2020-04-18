﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LiquidProjections;
using LiquidProjections.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;
using SqlStreamStore.Demo.Commands;
using SqlStreamStore.Demo.Commands.Account;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Demo.Persistence;
using SqlStreamStore.Demo.Projectors;
using SqlStreamStore.Demo.Serializers.Json;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Demo.Serializers.Messages.Account;
using SqlStreamStore.Demo.Services;
using SqlStreamStore.Streams;
using SqlStreamStore.MsSqlScripts;
using IEventStore = SqlStreamStore.Demo.Events.IEventStore;

namespace SqlStreamStore.Demo
{

    public interface IServiceCollectionConfiguration
    {
        void Configure(IServiceCollection serviceCollection);
    }


    public class ServiceCollectionConfiguration : IServiceCollectionConfiguration
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MsSqlStreamStoreSettings>(x => new MsSqlStreamStoreSettings("Server=localhost;Database=mailhelper-dev-db;Trusted_Connection=True;"));
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

        }
    }
    

    static class Program
    {
        private static BalanceProjection _balanceProjection;

        static async Task Main()
        {
            var serviceCollectionConfigurations = new IServiceCollectionConfiguration[]
            {
                new ServiceCollectionConfiguration()
            };
            var serviceProvider = GetServiceProvider(serviceCollectionConfigurations);

            var accountId = Guid.Parse("5af8872f-fd5c-4599-ac0d-29ddf400d823");
            var streamId = "Transaction:5af8872f-fd5c-4599-ac0d-29ddf400d823";

            var sqlStreamStore = serviceProvider.GetRequiredService<MsSqlStreamStore>();
            var checkResult = await sqlStreamStore.CheckSchema();
            if (!checkResult.IsMatch())
            {
                await sqlStreamStore.CreateSchema();
            }

            var accountService = serviceProvider.GetService<IAccountService>();
            
            var accountDbContext = new AccountDbContext();
            Func<DbContext> accountDbContextProvider = () => accountDbContext;


            
            var lastProcessed = 
            sqlStreamStore.SubscribeToStream(streamId, null, StreamMessageReceived);

            var transactionDispatcher = new Dispatcher(() => sqlStreamStore.SubscribeToStream());
            var accountProjector = new AccountProjector(accountDbContextProvider, transactionDispatcher);
            
            var readmodel = new AccountInfo();

            _balanceProjection = new BalanceProjection(sqlStreamStore, $"Transaction:5af8872f-fd5c-4599-ac0d-29ddf400d823", serviceProvider.GetService<IEventSerializer>(), readmodel);


            var key = string.Empty;
            while (key != "X")
            {
                Console.WriteLine("D: Deposit");
                Console.WriteLine("W: Withdrawal");
                Console.WriteLine("B: Balance");
                Console.WriteLine("T: Transactions");
                Console.WriteLine("X: Exit");
                Console.Write("> ");
                key = Console.ReadLine()?.ToUpperInvariant();
                Console.WriteLine();

                try
                {
                    switch (key)
                    {
                        case "D":
                            var depositAmount = GetAmount();
                            if (depositAmount.IsValid)
                            {
                                await accountService.Deposit(accountId, depositAmount.Amount, CancellationToken.None);
                                Console.WriteLine($"Deposited: {depositAmount.Amount:C}");
                            }
                            break;
                        case "W":
                            var withdrawalAmount = GetAmount();
                            if (withdrawalAmount.IsValid)
                            {
                                await accountService.Deposit(accountId, withdrawalAmount.Amount, CancellationToken.None);
                                Console.WriteLine($"Withdrawn: {withdrawalAmount.Amount:C} ()");
                            }
                            break;
                        case "B":
                            Console.WriteLine($"{readmodel.Balance.Amount} as of {readmodel.Balance.AsOf}");
                            break;
                        case "T":
                            foreach (var transaction in readmodel.Transactions)
                            {
                                Console.WriteLine($"{transaction.Type}: {transaction.Amount:C} @ {transaction.DateTime} ({transaction.TransactionId})");
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine();
            }
        }

        private static (decimal Amount, bool IsValid) GetAmount()
        {
            Console.Write("Amount: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                return (amount, true);
            }

            Console.WriteLine("Invalid Amount.");
            return (0, false);
        }

        private static IDisposable CreateSubscription(IStreamStore streamStore, string streamId)
        {

        }


        private static IServiceProvider GetServiceProvider(params IServiceCollectionConfiguration[] serviceCollectionConfigurations)
        {
            var serviceCollection = new ServiceCollection();
        
            foreach (var serviceCollectionConfiguration in serviceCollectionConfigurations)
            {
                serviceCollectionConfiguration.Configure(serviceCollection);
            }
            return serviceCollection.BuildServiceProvider();

        }
    }
}