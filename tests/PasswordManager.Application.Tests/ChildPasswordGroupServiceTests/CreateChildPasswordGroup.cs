using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.ChildPasswordGroupServiceTests;

public class CreateChildPasswordGroup : BaseChildPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldCreate_WhenEverythingIsValidAndAccountIsUser()
    {
        var parentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var passwordGroupDto = GetGivenPasswordGroupDto(parentPasswordGroup.Id);
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(parentPasswordGroup, parentPasswordGroup.Id);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(user, parentPasswordGroup))
            .ReturnsAsync(true);

        await Sut.CreateChildPasswordGroup(passwordGroupDto, user.Id);
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Once);
    }

    [Fact]
    public async Task ShouldNotCreate_WhenPasswordGroupDoesnNotExist()
    {
        var passwordGroupDto = GetGivenPasswordGroupDto(Guid.NewGuid());
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(null, passwordGroupDto.ParentPasswordGroupId);

        await Assert.ThrowsAsync<ParentPasswordGroupNotFoundException>(async () => await Sut.CreateChildPasswordGroup(passwordGroupDto, user.Id));
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Fact]
    public async Task ShouldNotCreate_WhenAccountDoesnNotExist()
    {
        var parentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var passwordGroupDto = GetGivenPasswordGroupDto(parentPasswordGroup.Id);
        var userId = Guid.NewGuid();

        var passwords = new List<Password>();

        MockGetAccount(null, userId);

        MockGetPasswordGroup(parentPasswordGroup, parentPasswordGroup.Id);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(It.IsAny<Account>(), parentPasswordGroup))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<AccountNotFoundException>(async () => await Sut.CreateChildPasswordGroup(passwordGroupDto, userId));
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Fact]
    public async Task ShouldNotCreate_WhenRolesAreIncompatible()
    {
        var parentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var passwordGroupDto = GetGivenPasswordGroupDto(parentPasswordGroup.Id);
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(parentPasswordGroup, parentPasswordGroup.Id);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(user, parentPasswordGroup))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<AccountPasswordGroupRolesInconsistencyException>(async () => await Sut.CreateChildPasswordGroup(passwordGroupDto, user.Id));
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    private PasswordGroupDto GetGivenPasswordGroupDto(Guid parentPasswordGroupId)
    {
        return new PasswordGroupDto()
        {
            Id = Guid.NewGuid(),
            AccessRoles = new List<RoleDto>(),
            Name = "TestName",
            ParentPasswordGroupId = parentPasswordGroupId,
            Passwords = new List<PasswordDto>()
        };
    }
}
