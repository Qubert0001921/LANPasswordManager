using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class PasswordGroupException : Exception
{
    public PasswordGroupException(string message): base(message)
    {
        
    }
}
