using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Tests.PasswordGroupHelperServiceTests;

public class HasAccountPasswordGroupRole : BasePasswordGroupHelperServiceTests
{
    [Fact]
    public async Task ShouldReturnTrue_WhenDataIsValid()
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();
        var someAdmin = Utils.GetValidAdmin();
        var roles = new List<RoleDto>
        {
            new RoleDto() { Id = Guid.NewGuid(), Name = "A" },
            new RoleDto() { Id = Guid.NewGuid(), Name = "B" },
            new RoleDto() { Id = Guid.NewGuid(), Name = "C" },
        };

        foreach (var role in roles)
        {
            account.AssignRole(new Role(role.Id, role.Name), someAdmin);
        }

        PasswordGroupServiceMock.Setup(x => x.GetAccessRolesByPasswordGroupId(passwordGroup.Id))
            .ReturnsAsync(roles);

        var actual = await Sut.HasAccountPasswordGroupRole(account, passwordGroup);

        Assert.True(actual);
    }

    [Fact]
    public async Task ShouldReturnFalse_WhenAccountLackOfRole()
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();
        var someAdmin = Utils.GetValidAdmin();
        var roles = new List<RoleDto>
        {
            new RoleDto() { Id = Guid.NewGuid(), Name = "A" },
            new RoleDto() { Id = Guid.NewGuid(), Name = "B" },
            new RoleDto() { Id = Guid.NewGuid(), Name = "C" },
        };

        PasswordGroupServiceMock.Setup(x => x.GetAccessRolesByPasswordGroupId(passwordGroup.Id))
            .ReturnsAsync(roles);

        var actual = await Sut.HasAccountPasswordGroupRole(account, passwordGroup);

        Assert.False(actual);
    }

    [Fact]
    public async Task ShouldReturnFalse_WhenPasswordGroupLackOfMatchRole()
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();
        var someAdmin = Utils.GetValidAdmin();
        var roles = new List<RoleDto>
        {
            new RoleDto() { Id = Guid.NewGuid(), Name = "A" },
        };

        PasswordGroupServiceMock.Setup(x => x.GetAccessRolesByPasswordGroupId(passwordGroup.Id))
            .ReturnsAsync(roles);

        var actual = await Sut.HasAccountPasswordGroupRole(account, passwordGroup);

        Assert.False(actual);
    }
}