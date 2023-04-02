using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Entities;

public class PasswordGroup
{
    public PasswordGroup(PasswordGroupType passwordGroupType, string name, List<Role> accessRoles, List<Password> passwords, PasswordGroup? parentPasswordGroup)
    {
        Name = name;
        _accessRoles = accessRoles;
        ParentPasswordGroup = parentPasswordGroup;
        _passwords = passwords;
        PasswordGroupType = passwordGroupType;
    }

    public static PasswordGroup CreateMainPasswordGroup(string name, List<Password> passwords, List<Role> accessRoles, Account creator)
    {
        if(!creator.IsAdmin)
        {
            throw new Exception("Only admin account can create main password group");
        }

        if(!accessRoles.Any())
        {
            throw new Exception("Main password group must have at least one role assigned");
        }

        return new PasswordGroup(PasswordGroupType.Main, name, accessRoles, passwords, null);
    }

    public static PasswordGroup CreateChildPasswordGroup(string name, List<Password> passwords, PasswordGroup parentPasswordGroup)
    {
        if(parentPasswordGroup is null)
        {
            throw new Exception("Child password group must have a parent");
        }

        return new PasswordGroup(PasswordGroupType.Child, name, new List<Role>(), passwords, parentPasswordGroup);
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
            throw new Exception("Cannot move main password group");
        }

        if(Id == parent.Id)
        {
            throw new Exception("Cannot move password group into itself");
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
            throw new Exception("Cannot add access role to child password group");
        }
        _accessRoles.Add(acessRole);
    }

    public void RemoveAccessRole(Role accessRole)
    {
        if(PasswordGroupType == PasswordGroupType.Child)
        {
            throw new Exception("Cannot remove access role from child password group");
        }
        _accessRoles.Remove(accessRole);
    }
}
