using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class MainPasswordGroupException : Exception
{
    public MainPasswordGroupException(string message): base(message)
    {
        
    }
}
