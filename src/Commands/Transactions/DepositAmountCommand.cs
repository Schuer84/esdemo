using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class DepositAmountCommand : TransactionCommand
    {
        public decimal Amount { get; set; }
    }
}
