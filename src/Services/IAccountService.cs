using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Services
{
    public interface IAccountService
    {
        Task<Guid> Register(string name, string email, CancellationToken cancellationToken);
        Task Withdraw(Guid accountId, decimal amount,CancellationToken cancellationToken);
        Task Deposit(Guid accountId, decimal amount, CancellationToken cancellationToken);

        Task<decimal> GetCurrentBalance(Guid accountId, CancellationToken cancellationToken);
        Task<IEnumerable<Transaction>> GetTransactions(Guid accountId, CancellationToken cancellationToken);
    }
}