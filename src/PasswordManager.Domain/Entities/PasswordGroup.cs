using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Domain.Entities;

public class PasswordGroup
{
    public PasswordGroup(Guid id, PasswordGroupType passwordGroupType, string name, List<Role> accessRoles, List<Password> passwords, PasswordGroup? parentPasswordGroup)
    {
        Id = id;
        Name = name;
        _accessRoles = accessRoles;
        ParentPasswordGroup = parentPasswordGroup;
        _passwords = passwords;
        PasswordGroupType = passwordGroupType;
    }

    public static PasswordGroup CreateMainPasswordGroup(Guid id, string name, List<Password> passwords, List<Role> accessRoles, Account creator)
    {
        if(!creator.IsAdmin)
        {
            throw new AdminPermissionRequiredException();
        }

        if(!accessRoles.Any())
        {
            throw new PasswordGroupException("Main password group must have at least one role assigned");
        }

        return new PasswordGroup(id, PasswordGroupType.Main, name, accessRoles, passwords, null);
    }

    public static PasswordGroup CreateChildPasswordGroup(Guid id, string name, List<Password> passwords, PasswordGroup parentPasswordGroup)
    {
        if(parentPasswordGroup is null)
        {
            throw new PasswordGroupException("Child password group must have a parent");
        }

        return new PasswordGroup(id, PasswordGroupType.Child, name, new List<Role>(), passwords, parentPasswordGroup);
    }

    public Guid Id { get; private init; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Role> AccessRoles => _accessRoles;
    private List<Role> _accessRoles;
    public PasswordGroup? ParentPasswordGroup { get; private set; }
    public IReadOnlyCollection<Password> Passwords => _passwords;
    private List<Password> _passwords;
    public PasswordGroupType PasswordGroupType { get; private set; }

    public void MovePasswordGroup(PasswordGroup parent)
    {
        if(PasswordGroupType == PasswordGroupType.Main)
        {
            throw new PasswordGroupException("Cannot move main password group");
        }

        if(Id == parent.Id)
        {
            throw new PasswordGroupException("Cannot move password group into itself");
        }

        ParentPasswordGroup = parent;
    }

    public void AddPassword(Password password)
    {
        _passwords.Add(password);
    }

    public void RemovePassword(Password password)
    {
        _passwords.Remove(password);
    }

    public void AddAccessRole(Role acessRole)
    {
        if(PasswordGroupType == PasswordGroupType.Child)
        {
            throw new PasswordGroupException("Cannot add access role to child password group");
        }
        _accessRoles.Add(acessRole);
    }

    public void RemoveAccessRole(Role accessRole)
    {
        if(PasswordGroupType == PasswordGroupType.Child)
        {
            throw new PasswordGroupException("Cannot remove access role from child password group");
        }
        _accessRoles.Remove(accessRole);
    }
}
