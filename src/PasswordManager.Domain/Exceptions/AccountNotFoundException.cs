using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException() : base("Account not found")
    {
        
    }
}
