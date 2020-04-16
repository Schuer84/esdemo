using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class RegisterAccountCommand : AccountCommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
