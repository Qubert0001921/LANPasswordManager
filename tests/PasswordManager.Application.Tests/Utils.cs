using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Tests;

public static class Utils
{
    public static Account GetValidAdmin()
    {
        return Account.CreateAdminAccount(
            Guid.NewGuid(),
            "adm_bartlomiej",
            "bartl",
            "lomiej",
            "bartek@example.com",
            "passwordsuper212131@@@@"
        );
    }

    public static Account GetValidUser()
    {
        return Account.CreateUserAccount(
            Guid.NewGuid(),
            "bartlomiej",
            "bartl",
            "lomiej",
            "bartekd@example.com",
            "passwordsuper212131@@@@",
            new List<Role>()
            {
                new Role(Guid.NewGuid(), "Rola")
            }
        );
    }

    public static Account GetValidAdmin(Guid id)
    {
        return Account.CreateAdminAccount(
            id,
            "adm_bartlomiej",
            "bartl",
            "lomiej",
            "bartek@example.com",
            "passwordsuper212131@@@@"
        );
    }

    public static Account GetValidUser(Guid id)
    {
        return Account.CreateUserAccount(
            id,
            "bartlomiej",
            "bartl",
            "lomiej",
            "bartekd@example.com",
            "passwordsuper212131@@@@",
            new List<Role>()
            {
                new Role(Guid.NewGuid(), "Rola")
            }
        );
    }

    public static Password GetValidPassword()
    {
        return new Password(Guid.NewGuid(), "8yrha98yh(&TGO(#&RT))");
    }

    public static PasswordGroup GetValidMainPasswordGroup()
    {
        return PasswordGroup.CreateMainPasswordGroup(
            Guid.NewGuid(),
            "PasswordGroup",
            new List<Password>()
            {
                Utils.GetValidPassword(),
                Utils.GetValidPassword(),
                Utils.GetValidPassword(),
            },
            new List<Role>()
            {
                GetValidRole()
            },
            GetValidAdmin()
        );
    }

    public static PasswordGroup GetValidChildPasswordGroup()
    {
        return PasswordGroup.CreateChildPasswordGroup(
            Guid.NewGuid(),
            "PasswordGroup",
            new List<Password>(),
            Utils.GetValidMainPasswordGroup()
        );
    }

    public static PasswordGroup GetValidChildPasswordGroup(Guid id)
    {
        return PasswordGroup.CreateChildPasswordGroup(
            id,
            "PasswordGroup",
            new List<Password>(),
            Utils.GetValidMainPasswordGroup()
        );
    }

    public static Role GetValidRole()
    {
        return new Role(Guid.NewGuid(), "Some role");
    }

    public static IEnumerable<PasswordGroup> GetValidChildPasswordGroupList(int count=5) 
        => Enumerable.Repeat<PasswordGroup>(GetValidChildPasswordGroup(), count);
}
