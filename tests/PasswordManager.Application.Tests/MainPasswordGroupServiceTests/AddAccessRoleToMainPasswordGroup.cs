using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public class AddAccessRoleToMainPasswordGroup : BaseMainPasswordGroupServiceTests
{
    public async Task ShouldAddAccessRoleToMainPasswordGroup_WhenRoleExistsAndAccountIsAdminAndPasswordGroupExists()
    {
        var passwordGroupId = Guid.NewGuid();
        var role = Utils.GetValidRole();
        var account = Utils.GetValidAdmin();

        var roleDto = new RoleDto()
        {
            Id = role.Id,
            Name = "qupa4kilo"
        };

        AccountRepoMock.Setup(x => x.GetByIdAsync(account.Id))
            .ReturnsAsync(account);
        
        RoleRepoMock.Setup(x => x.GetByIdAsync(role.Id))
            .ReturnsAsync(role);

        await Sut.AddAccessRoleToMainPasswordGroup(passwordGroupId, roleDto, account.Id);
    }
}
