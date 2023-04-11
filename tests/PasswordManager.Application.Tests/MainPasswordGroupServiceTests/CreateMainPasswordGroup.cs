using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PasswordManager.Domain.Repositories;
using PasswordManager.Application.Services.PasswordGroups;
using AutoMapper;
using PasswordManager.Domain.Entities;
using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public class CreateMainPasswordGroup : BaseMainPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldCreateMainPasswordGroup_WhenAccountIsAdmin()
    {
        var adminId = Guid.NewGuid();
        var admin = Utils.GetValidAdmin();

        var passwordGroupId = Guid.NewGuid();
        var passwordGroupDto = new PasswordGroupDto()
        {
            Id = passwordGroupId,
            AccessRoles = new List<RoleDto>()
            {
                new RoleDto()
            },
            Name = "passwordgroup",
            ParentPasswordGroupId = Guid.Empty,
            PasswordGroupType = PasswordGroupType.Main,
            Passwords = new List<PasswordDto>()
        };

        AccountRepoMock.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);
        

        await Sut.CreateMainPasswordGroup(passwordGroupDto, adminId);

        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()));
    }

    [Fact]
    public async Task ShouldThrowErrorAndNotCreate_WhenAccountIsNotAdmin()
    {
        var user = Utils.GetValidUser();
        var userId = user.Id;

        var passwordGroupId = Guid.NewGuid();
        var passwordGroupDto = new PasswordGroupDto()
        {
            Id = passwordGroupId,
            AccessRoles = new List<RoleDto>()
            {
                new RoleDto()
            },
            Name = "passwordgroup",
            ParentPasswordGroupId = Guid.Empty,
            PasswordGroupType = PasswordGroupType.Main,
            Passwords = new List<PasswordDto>()
        };

        AccountRepoMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<AdminPermissionRequiredException>(async () => await Sut.CreateMainPasswordGroup(passwordGroupDto, userId));
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Theory]
    [InlineData("", "'Name' musn't be empty")]
    [InlineData(")(&030958252)", "'Name' contains invalid characters")]
    [InlineData(null, "'Name' musn't be empty")]
    [InlineData("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW", "Length of 'Name' is invalid")]
    [InlineData("w", "Length of 'Name' is invalid")]
    public async Task ShouldThrowErrorAndNotCreate_WhenDataIsInvalid(string name, string message)
    {
        var admin = Utils.GetValidAdmin();
        var adminId = admin.Id;

        var passwordGroupId = Guid.NewGuid();
        var passwordGroupDto = new PasswordGroupDto()
        {
            Id = passwordGroupId,
            AccessRoles = new List<RoleDto>()
            {
                new RoleDto()
            },
            Name = name,
            ParentPasswordGroupId = Guid.Empty,
            PasswordGroupType = PasswordGroupType.Main,
            Passwords = new List<PasswordDto>()
        };

        AccountRepoMock.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var ex = await Assert.ThrowsAsync<ValidationProcessException>(async () => await Sut.CreateMainPasswordGroup(passwordGroupDto, adminId));
        Assert.Equal(message, ex.Errors.First().Message);

        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }
}
