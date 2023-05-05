using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class AccountException : Exception
{
    public AccountException(string message) : base(message)
    {
        
    }
}
