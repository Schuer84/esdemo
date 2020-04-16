using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
            serviceCollection.AddSingleton<IEventStore, EventStore>();
            serviceCollection.AddSingleton<AggregateRepository>();
            serviceCollection.AddTransient<AccountAggregate>();
            serviceCollection.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
            serviceCollection.AddSingleton<IMessageSerializer, DepositMessageSerializer>();
            serviceCollection.AddSingleton<IMessageSerializer, WithdrawnMessageSerializer>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var streamId = new StreamId($"Account:{Guid.NewGuid()}");

            var sqlStreamStore = serviceProvider.GetRequiredService<MsSqlStreamStore>();

            var checkResult = await sqlStreamStore.CheckSchema();
            if (!checkResult.IsMatch())
            {
                await sqlStreamStore.CreateSchema();
            }

            var aggregateRepository = serviceProvider.GetRequiredService<AggregateRepository>(); 
            var aggregate = await aggregateRepository.GetById<AccountAggregate>(streamId, CancellationToken.None);
            

            _balanceProjection = new BalanceProjection(_streamStore, streamId);
                
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
                
                switch (key)
                {
                    case "D":
                        var depositAmount = GetAmount();
                        if (depositAmount.IsValid)
                        {
                            var depositTrx = await aggregate.DepositAsync(depositAmount.Amount, CancellationToken.None);
                            Console.WriteLine($"Deposited: {depositAmount.Amount:C} ({depositTrx})");
                        }
                        break;
                    case "W":
                        var withdrawalAmount = GetAmount();
                        if (withdrawalAmount.IsValid)
                        {
                            var withdrawalTrx = await aggregate.WithdrawAsync(withdrawalAmount.Amount, CancellationToken.None);
                            Console.WriteLine($"Withdrawn: {withdrawalAmount.Amount:C} ({withdrawalTrx})");
                        }
                        break;
                    case "B":
                        Balance();
                        break;
                    case "T":
                        await _account.Transactions();
                        break;
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
        
        private static void Balance()
        {
            Console.WriteLine($"Balance: {_balanceProjection.Balance.Amount:C} as of {_balanceProjection.Balance.AsOf}");
        }
    }
}