using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Tests.ChildPasswordGroupServiceTests;

public class CreateChildPasswordGroup : BaseChildPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldCreate_WhenEverythingIsValidAndAccountIsUser()
    {
        var parentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var passwordGroupDto = new PasswordGroupDto()
        {
            Id = Guid.NewGuid(),
            AccessRoles = new List<RoleDto>(),
            Name = "TestName",
            ParentPasswordGroupId = parentPasswordGroup.Id,
            PasswordGroupType = PasswordGroupType.Child,
            Passwords = new List<PasswordDto>()
        };
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();
        var expected = PasswordGroup.CreateChildPasswordGroup(
            passwordGroupDto.Id,
            passwordGroupDto.Name,
            passwords,
            parentPasswordGroup
        );

        AccountRepoMock.Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(parentPasswordGroup.Id))
            .ReturnsAsync(parentPasswordGroup);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(user, parentPasswordGroup))
            .ReturnsAsync(true);

        await Sut.CreateChildPasswordGroup(passwordGroupDto, user.Id);
        PasswordGroupRepoMock.Verify(x => x.CreateOneAsync(expected), Times.Once);
    }
}
