using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class ParentPasswordGroupNotFoundException : Exception
{
    public ParentPasswordGroupNotFoundException() : base("Parent password group not found")
    {
        
    }
}
