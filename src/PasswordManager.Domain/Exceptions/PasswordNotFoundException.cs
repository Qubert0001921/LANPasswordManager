using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class PasswordNotFoundException : Exception
{
    public PasswordNotFoundException() : base("Password not found exception")
    {
        
    }
}
