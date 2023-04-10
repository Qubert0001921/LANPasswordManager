using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class AdminPermissionRequiredException : Exception
{
    public AdminPermissionRequiredException() : base("Admin privileges are required to that operation")
    {
        
    }
}
