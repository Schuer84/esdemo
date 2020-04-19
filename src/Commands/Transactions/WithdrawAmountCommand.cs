using System.Collections.Generic;
using System.Text;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class WithdrawAmountCommand : TransactionCommand
    {
        public decimal Amount { get; set; }
    }
}
