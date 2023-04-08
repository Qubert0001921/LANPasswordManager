using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class PasswordGroupNotFoundException : Exception
{
    public PasswordGroupNotFoundException() : base("Password group not found")
    {
        
    }
}
