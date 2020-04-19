using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Commands;
using SqlStreamStore.Demo.Commands.Account;
using SqlStreamStore.Demo.Persistence.Entities;
using SqlStreamStore.Demo.Queries;
using SqlStreamStore.Demo.Queries.Account;

namespace SqlStreamStore.Demo.Services
{
    public class AccountService : IAccountService
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public AccountService(ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
        }

        public async Task<Guid> Register(string name, string email, CancellationToken cancellationToken)
        {
            var accountId = Guid.NewGuid();
            var registerAccountCommand = new RegisterAccountCommand()
            {
                Name = name,
                Email = email,
                AccountId = accountId
            };
            await _commandHandler.Handle(registerAccountCommand, cancellationToken);
            return accountId;
        }

        public async Task Withdraw(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var withdrawAmountCommand = new WithdrawAmountCommand()
            {
                AccountId = accountId,
                Amount = amount
            };
            await _commandHandler.Handle(withdrawAmountCommand, cancellationToken);
        }

        public async Task Deposit(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var withdrawAmountCommand = new DepositAmountCommand()
            {
                AccountId = accountId,
                Amount = amount
            };
            await _commandHandler.Handle(withdrawAmountCommand, cancellationToken);
        }

        public async Task<decimal> GetCurrentBalance(Guid accountId, CancellationToken cancellationToken)
        {
            var query = new GetAccountTransactionsQuery()
            {
                AccountId = accountId
            };
            return await _queryHandler.Query<Transaction, decimal>(query, cancellationToken);
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(Guid accountId, CancellationToken cancellationToken)
        {
            var query = new GetAccountTransactionsQuery()
            {
                AccountId = accountId
            };
            return await _queryHandler.Query<Transaction, IEnumerable<Transaction>>(query, cancellationToken);
        }
    }
}
