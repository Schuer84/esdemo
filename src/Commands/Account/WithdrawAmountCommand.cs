using System.Collections.Generic;
using System.Text;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class WithdrawAmountCommand : AccountCommand
    {
        public decimal Amount { get; set; }
    }
}
