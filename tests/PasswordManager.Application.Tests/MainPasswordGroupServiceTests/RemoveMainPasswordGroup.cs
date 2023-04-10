using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public class RemoveMainPasswordGroup : BaseMainPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldRemoveMainPasswordGroup_WhenAccountIsAdminAndPasswordGroupExists()
    {
        var admin = Utils.GetValidAdmin();

        var passwordGroup = Utils.GetValidMainPasswordGroup();

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        await Sut.RemoveMainPasswordGroup(passwordGroup.Id, admin.Id);

        PasswordGroupRepoMock.Verify(x => x.RemoveOneByIdAsync(passwordGroup.Id));
    }

    [Fact]
    public async Task ShouldNotRemoveMainPasswordGroupAndThrowError_WhenAccountIsNotAdminAndPasswordGroupExists()
    {
        var user = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidMainPasswordGroup();

        AccountRepoMock.Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);


        await Assert.ThrowsAsync<AdminPermissionRequiredException>(async () => await Sut.RemoveMainPasswordGroup(passwordGroup.Id, user.Id));
        PasswordGroupRepoMock.Verify(x => x.RemoveOneByIdAsync(passwordGroup.Id), Times.Never);
    }

    [Fact]
    public async Task ShouldNotRemoveMainPasswordGroupAndThrowError_WhenAccountIsAdminAndPasswordGroupDoesNotExist()
    {
        var admin = Utils.GetValidAdmin();
        var passwordGroupId = Guid.NewGuid();

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroupId))
            .ReturnsAsync(() => null);

        await Assert.ThrowsAsync<PasswordGroupNotFoundException>(async () => await Sut.RemoveMainPasswordGroup(passwordGroupId, admin.Id));
        PasswordGroupRepoMock.Verify(x => x.RemoveOneByIdAsync(passwordGroupId), Times.Never);
    }

    [Fact]
    public async Task ShouldRemoveAllChildrenOfMainPasswordGroup_WhenAccountIsAdminAndPasswordGroupExists()
    {
        var admin = Utils.GetValidAdmin();
        var passwordGroup = Utils.GetValidMainPasswordGroup();

        var allChildren = new List<PasswordGroup>()
        {
            Utils.GetValidChildPasswordGroup(),
            Utils.GetValidChildPasswordGroup(),
            Utils.GetValidChildPasswordGroup(),
        };
        var allChildrenIds = allChildren.Select(x => x.Id);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupHelperMock.Setup(x => x.GetAllChildrenOfPasswordGroup(passwordGroup))
            .ReturnsAsync(allChildren);

        await Sut.RemoveMainPasswordGroup(passwordGroup.Id, admin.Id);


        PasswordGroupRepoMock.Verify(x => x.RemoveRangeByIdsAsync(allChildrenIds), Times.Once);
    }

    
    [Fact]
    public async Task ShouldRemoveAllPasswordsOfMainPasswordGroup_WhenAccountIsAdminAndPasswordGroupExists()
    {
        var admin = Utils.GetValidAdmin();
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var passwordIds = passwordGroup.Passwords.Select(x => x.Id);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupHelperMock.Setup(x => x.GetAllPasswordIdsOfPasswordGroup(passwordGroup, It.IsAny<IEnumerable<PasswordGroup>>()))
            .Returns(passwordIds);

        await Sut.RemoveMainPasswordGroup(passwordGroup.Id, admin.Id);

        PasswordRepoMock.Verify(x => x.RemoveRangeByIdsAsync(passwordIds), Times.Once);
    }
}
