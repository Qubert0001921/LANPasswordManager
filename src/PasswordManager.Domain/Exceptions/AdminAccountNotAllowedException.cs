using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class AdminAccountNotAllowedException : Exception
{
    public AdminAccountNotAllowedException() : base("Admin account is not allowed to that operation")
    {
        
    }
}
