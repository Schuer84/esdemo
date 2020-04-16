using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;
using SqlStreamStore.Demo.Commands;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Demo.Serializers.Json;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Demo.Serializers.Messages.Account;
using SqlStreamStore.Demo.Services;
using SqlStreamStore.Streams;
using SqlStreamStore.MsSqlScripts;

namespace SqlStreamStore.Demo
{
    static class Program
    {
        private static BalanceProjection _balanceProjection;

        static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
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

            serviceCollection.AddTransient<ICommandHandler, CommandHandler>();
            serviceCollection.AddTransient<IAccountService, AccountService>();


            var serviceProvider = serviceCollection.BuildServiceProvider();
            var streamId = new StreamId($"Account:5af8872f-fd5c-4599-ac0d-29ddf400d823");

            var sqlStreamStore = serviceProvider.GetRequiredService<MsSqlStreamStore>();

            var checkResult = await sqlStreamStore.CheckSchema();
            if (!checkResult.IsMatch())
            {
                await sqlStreamStore.CreateSchema();
            }

            var aggregateRepository = serviceProvider.GetRequiredService<AggregateRepository>(); 
            var aggregate = await aggregateRepository.GetById<AccountAggregate>(streamId, CancellationToken.None);
            var readmodel = new AccountInfo();

            _balanceProjection = new BalanceProjection(sqlStreamStore, streamId, serviceProvider.GetService<IEventSerializer>(), readmodel);
                
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
                                var depositTrx = aggregate.Deposit(depositAmount.Amount);
                                Console.WriteLine($"Deposited: {depositAmount.Amount:C} ({depositTrx})");
                            }
                            break;
                        case "W":
                            var withdrawalAmount = GetAmount();
                            if (withdrawalAmount.IsValid)
                            {
                                var withdrawalTrx = aggregate.Withdraw(withdrawalAmount.Amount);
                                Console.WriteLine($"Withdrawn: {withdrawalAmount.Amount:C} ({withdrawalTrx})");
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
    }
}