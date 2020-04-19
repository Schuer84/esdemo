using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LiquidProjections;
using LiquidProjections.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlStreamStore.Demo.Persistence;
using SqlStreamStore.Demo.Projectors;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Demo.Services;
using SqlStreamStore.Streams;
using SqlStreamStore.MsSqlScripts;

namespace SqlStreamStore.Demo
{
    static class Program
    {

        static async Task Main()
        {
            var serviceCollectionConfigurations = new IServiceCollectionConfiguration[]
            {
                new ServiceCollectionConfiguration()
            };
            var serviceProvider = GetServiceProvider(serviceCollectionConfigurations);

            var accountId = Guid.Parse("5af8872f-fd5c-4599-ac0d-29ddf400d823");
            var streamId = "Transaction:5af8872f-fd5c-4599-ac0d-29ddf400d823";
            Database.SetInitializer(new CreateDatabaseIfNotExists<AccountDbContext>());

            var accountDbContext = serviceProvider.GetService<AccountDbContext>();
                accountDbContext.Database.CreateIfNotExists();

            var sqlStreamStore = serviceProvider.GetRequiredService<MsSqlStreamStore>();
            var checkResult = await sqlStreamStore.CheckSchema();
            if (!checkResult.IsMatch())
            {
                await sqlStreamStore.CreateSchema();
            }

            var accountService = serviceProvider.GetService<IAccountService>();
            var eventSerializer = serviceProvider.GetService<IEventSerializer>();
            
            
            Func<DbContext> accountDbContextProvider = () => serviceProvider.GetService<AccountDbContext>();


            var subscriber = new StreamStoreSubscriber(sqlStreamStore, streamId, eventSerializer);
            var transactionDispatcher = new Dispatcher(subscriber.Subscribe);
            var accountProjector = new AccountProjector(accountDbContextProvider, transactionDispatcher);
                accountProjector.Start();
            //var readmodel = new AccountInfo();

            //_balanceProjection = new BalanceProjection(sqlStreamStore, $"Transaction:5af8872f-fd5c-4599-ac0d-29ddf400d823", serviceProvider.GetService<IEventSerializer>(), readmodel);


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
                            var balance = await accountService.GetCurrentBalance(accountId, CancellationToken.None);
                            Console.WriteLine($"You currently have: {balance} ");
                            break;
                        case "T":
                            var transactions = await accountService.GetTransactions(accountId, CancellationToken.None);
                            foreach (var transaction in transactions)
                            {
                                Console.WriteLine($"{transaction.Type}: {transaction.Amount:C} @ {transaction.CreatedOn} ({transaction.Id})");
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