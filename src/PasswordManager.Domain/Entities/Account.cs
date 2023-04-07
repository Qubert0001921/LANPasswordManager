using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Entities;

public class Account
{
    private Account(Guid id, string login, string firstName, string lastName, string email, string password, List<Role> roles, bool isAdmin)
    {
        Id = id;
        Login = login;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        _roles = roles;
        IsAdmin = isAdmin;
    }

    public static Account CreateAdminAccount(Guid id, string login, string firstName, string lastName, string email, string password)
    {
        return new Account(
            id,
            login,
            firstName,
            lastName,
            email,
            password,
            new List<Role>(),
            true
        );
    }

    public static Account CreateUserAccount(Guid id, string login, string firstName, string lastName, string email, string password, IEnumerable<Role> roles)
    {
        if(!roles.Any())
        {
            throw new Exception("User account must have at least one role");
        }

        return new Account(
            id,
            login,
            firstName,
            lastName,
            email,
            password,
            roles.ToList(),
            false
        );
    }

    public Guid Id { get; private init; }
    public string Login { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles;
    private List<Role> _roles;
    public bool IsAdmin { get; private set; }

    // public void AssignRole(Role role, Account account)
    // {
    //     if(!account.IsAdmin)
    //     {
    //         throw new Exception("Only administrator account can assign role to an account");
    //     }

    //     if(IsAdmin)
    //     {
    //         throw new Exception("Cannot assign role to an admin account");
    //     }

    //     bool alreadyHasRole = _roles.Where(accountRole => accountRole.Id == role.Id).FirstOrDefault() is not null;
        
    //     if(alreadyHasRole) 
    //     {
    //         throw new Exception("Cannot assign already assigned role to the user account");
    //     }

    //     _roles.Add(role);
    // }

    // public void RemoveRoleAssignment(Role role, Account account)
    // {
    //     if(!account.IsAdmin)
    //     {
    //         throw new Exception("Only administrator account can remove role assignement from a different account");
    //     }

    //     bool hasRole = _roles.Where(accountRole => accountRole.Id == role.Id).FirstOrDefault() is not null;

    //     if(!hasRole)
    //     {
    //         throw new Exception("Cannot remove assignment of unassigned role");
    //     }

    //     _roles.Remove(role);
    // }
}
