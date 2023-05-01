using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public class AddAccessRoleToMainPasswordGroup : BaseMainPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldAddAccessRoleToMainPasswordGroup_WhenRoleExistsAndAccountIsAdminAndPasswordGroupExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var role = Utils.GetValidRole();
        var admin = Utils.GetValidAdmin();

        PasswordGroupHelperMock.Setup(x => x.GetAndValidPasswordGroup(passwordGroup.Id, PasswordGroupType.Main))
            .ReturnsAsync(passwordGroup);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        PasswordGroupRepoMock.Setup(x => x.UpdateOneAsync(passwordGroup));

        await Sut.AddAccessRoleToMainPasswordGroup(passwordGroup.Id, role.Id, admin.Id);

        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup));
    }

    [Fact]
    public async Task ShouldNotAddAccessRoleAndThrow_WhenRoleDoesNotExistAndAccountIsAdminAndPasswordGroupExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var roleId = Guid.NewGuid();
        var admin = Utils.GetValidAdmin();

        PasswordGroupHelperMock.Setup(x => x.GetAndValidPasswordGroup(passwordGroup.Id, PasswordGroupType.Main))
            .ReturnsAsync(passwordGroup);

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync(() => null);

        await Assert.ThrowsAsync<RoleNotFoundException>(async () => await Sut.AddAccessRoleToMainPasswordGroup(passwordGroup.Id, roleId, admin.Id));

        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup), Times.Never);
    }

    [Fact]
    public async Task ShouldNotAddAccessRoleAndThrow_WhenRoleExistsAndAccountIsNotAdminAndPasswordGroupExists()
    {
        var passwordGroup = Utils.GetValidMainPasswordGroup();
        var role = Utils.GetValidRole();
        var user = Utils.GetValidUser();

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        AccountRepoMock.Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        await Assert.ThrowsAsync<AdminPermissionRequiredException>(async () => await Sut.AddAccessRoleToMainPasswordGroup(passwordGroup.Id, role.Id, user.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(passwordGroup), Times.Never);
    }

    [Fact]
    public async Task ShouldNotAddAccessRoleAndThrow_WhenRoleExistsAndAccountIsAdminAndPasswordGroupDoesNotExist()
    {
        var passwordGroupId = Guid.NewGuid();
        var role = Utils.GetValidRole();
        var admin = Utils.GetValidAdmin();

        PasswordGroupHelperMock.Setup(x => x.GetAndValidPasswordGroup(passwordGroupId, PasswordGroupType.Main))
            .ThrowsAsync(new PasswordGroupNotFoundException());

        AccountRepoMock.Setup(x => x.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        await Assert.ThrowsAsync<PasswordGroupNotFoundException>(async () => await Sut.AddAccessRoleToMainPasswordGroup(passwordGroupId, role.Id, admin.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }
}
