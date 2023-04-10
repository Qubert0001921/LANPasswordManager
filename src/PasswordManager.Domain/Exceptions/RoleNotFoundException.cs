using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException() : base("Role not found")
    {
        
    }
}
