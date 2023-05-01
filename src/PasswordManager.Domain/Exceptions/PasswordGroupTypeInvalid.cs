using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class PasswordGroupTypeInvalidException : Exception
{
    public PasswordGroupTypeInvalidException() : base("Password group type is invalid")
    {
        
    }
}
