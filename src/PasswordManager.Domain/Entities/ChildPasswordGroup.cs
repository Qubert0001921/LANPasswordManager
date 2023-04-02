using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Entities;

public class ChildPasswordGroup
{
    public ChildPasswordGroup(Guid id, PasswordGroup passwordGroup, PasswordGroup parentPasswordGroup)
    {
        if(id == null) throw new NullReferenceException();
        if(passwordGroup is null) throw new NullReferenceException();

        if(parentPasswordGroup.Id == passwordGroup.Id)
        {
            throw new Exception("Child password group cannot have parent password group as itself");
        }

        Id = id;
        PasswordGroup = passwordGroup;
        ParentPasswordGroup = parentPasswordGroup;
    }

    public Guid Id { get; private init; }
    public PasswordGroup PasswordGroup { get; private set; }
    public PasswordGroup ParentPasswordGroup { get; private set; }

    public void MovePasswordGroup(Account account, PasswordGroup parentPasswordGroup)
    {
        if(PasswordGroup.Id == parentPasswordGroup.Id)
        {
            throw new Exception("Cannot move password group into itself");
        }

        ParentPasswordGroup = parentPasswordGroup;
    }
}
