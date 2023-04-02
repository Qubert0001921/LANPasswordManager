using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Entities;

public class MainPasswordGroup
{
    public MainPasswordGroup(Guid id, List<Role> accessRoles, PasswordGroup passwordGroup, Account creator)
    {
        if(id == null) throw new NullReferenceException();
        if(accessRoles is null) throw new NullReferenceException();

        if(!creator.IsAdmin)
        {
            throw new Exception("Only an administrator account can create main password group");
        }

        if(accessRoles.Count == 0)
        {
            throw new Exception("Main group must have at least one role");
        }

        Id = id;
        AccessRoles = accessRoles;
        PasswordGroup = passwordGroup;
    }

    public Guid Id { get; private init; }
    public List<Role> AccessRoles { get; private set; }
    public PasswordGroup PasswordGroup { get; private set; }
}
