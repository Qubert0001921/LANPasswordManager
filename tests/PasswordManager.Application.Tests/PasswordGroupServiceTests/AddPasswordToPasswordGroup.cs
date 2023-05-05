using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.PasswordGroupServiceTests;

public class AddPasswordToPasswordGroup : BasePasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldAdd_WhenDataIsValid()
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        var passwordGroupDto = GetPasswordGroupDto(passwordGroup);
        var accountDto = GetAccontDto(account);
        var passwordDto = GetPasswordDto();

        AccountRepoMock.Setup(x => x.GetByIdAsync(accountDto.Id))
            .ReturnsAsync(account);

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroupDto.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(account, passwordGroup))
            .ReturnsAsync(true);

        await Sut.AddPasswordToPasswordGroup(passwordGroupDto, passwordDto, accountDto);

        PasswordRepoMock.Verify(x => x.CreateOneAsync(It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Once);
        PasswordGroupRepoMock.Verify(x => x.AddPasswordToPasswordGroupAsync(It.Is<PasswordGroup>(x => x.Id == passwordGroupDto.Id), It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Once);
    }

    [Fact]
    public async Task ShouldThrow_WhenAdmin()
    {
        var account = Utils.GetValidAdmin();
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        var passwordGroupDto = GetPasswordGroupDto(passwordGroup);
        var accountDto = GetAccontDto(account);
        var passwordDto = GetPasswordDto();

        AccountRepoMock.Setup(x => x.GetByIdAsync(accountDto.Id))
            .ReturnsAsync(account);

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroupDto.Id))
            .ReturnsAsync(passwordGroup);

        await Assert.ThrowsAsync<AdminAccountNotAllowedException>(async () => await Sut.AddPasswordToPasswordGroup(passwordGroupDto, passwordDto, accountDto));

        PasswordRepoMock.Verify(x => x.CreateOneAsync(It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
        PasswordGroupRepoMock.Verify(x => x.AddPasswordToPasswordGroupAsync(It.Is<PasswordGroup>(x => x.Id == passwordGroupDto.Id), It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
    }

    [Fact]
    public async Task ShouldThrow_WhenRolesDontMatch()
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        var passwordGroupDto = GetPasswordGroupDto(passwordGroup);
        var accountDto = GetAccontDto(account);
        var passwordDto = GetPasswordDto();

        AccountRepoMock.Setup(x => x.GetByIdAsync(accountDto.Id))
            .ReturnsAsync(account);

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroupDto.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(account, passwordGroup))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<AccountPasswordGroupRolesInconsistencyException>(async () => await Sut.AddPasswordToPasswordGroup(passwordGroupDto, passwordDto, accountDto));

        PasswordRepoMock.Verify(x => x.CreateOneAsync(It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
        PasswordGroupRepoMock.Verify(x => x.AddPasswordToPasswordGroupAsync(It.Is<PasswordGroup>(x => x.Id == passwordGroupDto.Id), It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("           ")]
    public async Task ShouldThrow_PasswordIsInvalid(string passwordCipher)
    {
        var account = Utils.GetValidUser();
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        var passwordGroupDto = GetPasswordGroupDto(passwordGroup);
        var accountDto = GetAccontDto(account);
        var passwordDto = new PasswordDto()
        {
            Id = Guid.NewGuid(),
            PasswordCipher = passwordCipher
        };

        AccountRepoMock.Setup(x => x.GetByIdAsync(accountDto.Id))
            .ReturnsAsync(account);

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroupDto.Id))
            .ReturnsAsync(passwordGroup);

        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(account, passwordGroup))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<ValidationProcessException>(async () => await Sut.AddPasswordToPasswordGroup(passwordGroupDto, passwordDto, accountDto));

        PasswordRepoMock.Verify(x => x.CreateOneAsync(It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
        PasswordGroupRepoMock.Verify(x => x.AddPasswordToPasswordGroupAsync(It.Is<PasswordGroup>(x => x.Id == passwordGroupDto.Id), It.Is<Password>(x => x.Id == passwordDto.Id)), Times.Never);
    }
}
