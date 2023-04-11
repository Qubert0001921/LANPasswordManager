using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class ChildPasswordGroupException : Exception
{
    public ChildPasswordGroupException(string msg): base(msg)
    {
        
    }
}
