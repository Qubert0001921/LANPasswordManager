using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Exceptions;

public class AccountPasswordGroupRolesInconsistencyException : Exception
{
    public AccountPasswordGroupRolesInconsistencyException() : base("Account doesn't have password group role")
    {
        
    }
}
