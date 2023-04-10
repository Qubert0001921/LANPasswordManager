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
}
