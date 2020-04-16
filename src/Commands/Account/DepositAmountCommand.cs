using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class DepositAmountCommand : AccountCommand
    {
        public decimal Amount { get; set; }
    }
}
