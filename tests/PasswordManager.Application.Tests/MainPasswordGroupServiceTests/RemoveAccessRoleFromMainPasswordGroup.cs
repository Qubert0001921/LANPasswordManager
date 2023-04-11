using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public class RemoveAccessRoleFromMainPasswordGroup : BaseMainPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldRemoveAccessRole_WhenPasswordGroupExistsAndAccountIsAdminAndRoleExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var role = Utils.GetValidRole();
        var admin = Utils.GetValidAdmin();

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);
        
        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupRepoMock.Setup(x => x.UpdateOneAsync(passwordGroup));

        await Sut.RemoveAccessRoleFromMainPasswordGroup(passwordGroup.Id, role.Id, admin.Id);

        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup), Times.Once);
    }

    [Fact]
    public async Task ShouldNotRemoveAccessRoleAndThrow_WhenRoleDoesNotExistAndAccountIsAdminAndPasswordGroupExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var roleId = Guid.NewGuid();
        var admin = Utils.GetValidAdmin();

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync(() => null);

        await Assert.ThrowsAsync<RoleNotFoundException>(async () => await Sut.RemoveAccessRoleFromMainPasswordGroup(passwordGroup.Id, roleId, admin.Id));

        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup), Times.Never);
    }

    [Fact]
    public async Task ShouldNotRemoveAccessRoleAndThrow_WhenRoleExistsAndAccountIsNotAdminAndPasswordGroupExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var role = Utils.GetValidRole();
        var user = Utils.GetValidUser();

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        AccountRepoMock.Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        await Assert.ThrowsAsync<AdminPermissionRequiredException>(async () => await Sut.RemoveAccessRoleFromMainPasswordGroup(passwordGroup.Id, role.Id, user.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup), Times.Never);
    }

    [Fact]
    public async Task ShouldNotRemoveAccessRoleAndThrow_WhenRoleExistsAndAccountIsAdminAndPasswordGroupDoesNotExist()
    {
        var passwordGroupId = Guid.NewGuid();
        var role = Utils.GetValidRole();
        var admin = Utils.GetValidAdmin();

        PasswordGroupRepoMock.Setup(x => x.GetMainPasswordGroupByIdAsync(passwordGroupId))
            .ReturnsAsync(() => null);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        await Assert.ThrowsAsync<PasswordGroupNotFoundException>(async () => await Sut.RemoveAccessRoleFromMainPasswordGroup(passwordGroupId, role.Id, admin.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }
}
