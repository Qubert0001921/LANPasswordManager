using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.PasswordGroupHelperServiceTests;

public class GetAndValidPasswordGroup : BasePasswordGroupHelperServiceTests
{
    private PasswordGroup childPasswordGroup = Utils.GetValidChildPasswordGroup();
    private PasswordGroup mainPasswordGroup = Utils.GetValidMainPasswordGroup();

    [Fact]
    public async Task ShouldGet_WhenDataIsValid()
    {
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        var result = await Sut.GetAndValidPasswordGroup(passwordGroup.Id, passwordGroup.PasswordGroupType);
        Assert.Equal(result.Id, passwordGroup.Id);
    }

    [Fact]
    public async Task ShouldThrowNotFound_WhenPasswordGroupNotExists()
    {
        var passwordGroup = Utils.GetValidChildPasswordGroup();

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroup.Id))
            .ReturnsAsync(() => null);

        await Assert.ThrowsAsync<PasswordGroupNotFoundException>(async () => await Sut.GetAndValidPasswordGroup(passwordGroup.Id, passwordGroup.PasswordGroupType));
    }

    [Fact]
    public async Task ShouldThrowTypesInvalid_WhenTypesAreInvalid()
    {
        var passwordGroup = Utils.GetValidChildPasswordGroup();
        var passwordGroupType = PasswordGroupType.Main;

        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(passwordGroup.Id))
            .ReturnsAsync(passwordGroup);

        await Assert.ThrowsAsync<PasswordGroupTypeInvalidException>(async () => await Sut.GetAndValidPasswordGroup(passwordGroup.Id, passwordGroupType));
    }
}

