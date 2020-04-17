using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Commands;
using SqlStreamStore.Demo.Commands.Account;

namespace SqlStreamStore.Demo.Services
{
    public interface IAccountService
    {
        Task<Guid> Register(string name, string email, CancellationToken cancellationToken);
        Task Withdraw(Guid accountId, decimal amount,CancellationToken cancellationToken);
        Task Deposit(Guid accountId, decimal amount, CancellationToken cancellationToken);
    }

    public class AccountService : IAccountService
    {
        private readonly ICommandHandler _commandHandler;
        public AccountService(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
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
    }
}
