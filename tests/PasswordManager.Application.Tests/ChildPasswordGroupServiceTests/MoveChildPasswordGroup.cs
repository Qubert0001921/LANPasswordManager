using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;

namespace PasswordManager.Application.Tests.ChildPasswordGroupServiceTests;

public class MoveChildPasswordGroup : BaseChildPasswordGroupServiceTests
{
    [Fact]
    public async Task ShouldMove_WhenDataIsValid()
    {
        var newParentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var childPasswordGroup = Utils.GetValidChildPasswordGroup();
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(newParentPasswordGroup, newParentPasswordGroup.Id);

        MockGetValidPasswordGroup(childPasswordGroup, childPasswordGroup.Id);

        MockDoRolesMatch(true, user, newParentPasswordGroup);

        MockDoRolesMatch(true, user, childPasswordGroup);

        await Sut.MoveChildPasswordGroup(childPasswordGroup.Id, newParentPasswordGroup.Id, user.Id);
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrow_WhenParentRolesAreInvalid()
    {
        var newParentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var childPasswordGroup = Utils.GetValidChildPasswordGroup();
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(newParentPasswordGroup, newParentPasswordGroup.Id);

        MockGetValidPasswordGroup(childPasswordGroup, childPasswordGroup.Id);

        MockDoRolesMatch(false, user, newParentPasswordGroup);

        MockDoRolesMatch(true, user, childPasswordGroup);

        await Assert.ThrowsAsync<AccountPasswordGroupRolesInconsistencyException>(async () => await Sut.MoveChildPasswordGroup(childPasswordGroup.Id, newParentPasswordGroup.Id, user.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Fact]
    public async Task ShouldThrow_WhenChildRolesAreInvalid()
    {
        var newParentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var childPasswordGroup = Utils.GetValidChildPasswordGroup();
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(newParentPasswordGroup, newParentPasswordGroup.Id);

        MockGetValidPasswordGroup(childPasswordGroup, childPasswordGroup.Id);

        MockDoRolesMatch(true, user, newParentPasswordGroup);

        MockDoRolesMatch(false, user, childPasswordGroup);

        await Assert.ThrowsAsync<AccountPasswordGroupRolesInconsistencyException>(async () => await Sut.MoveChildPasswordGroup(childPasswordGroup.Id, newParentPasswordGroup.Id, user.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Fact]
    public async Task ShouldThrow_WhenAccountDoesNotExist()
    {
        var newParentPasswordGroup = Utils.GetValidChildPasswordGroup();
        var childPasswordGroup = Utils.GetValidChildPasswordGroup();
        var userId = Guid.NewGuid();

        var passwords = new List<Password>();

        MockGetAccount(null, userId);

        MockGetPasswordGroup(newParentPasswordGroup, newParentPasswordGroup.Id);

        MockGetValidPasswordGroup(childPasswordGroup, childPasswordGroup.Id);

        MockDoRolesMatch(true, It.IsAny<Account>(), newParentPasswordGroup);

        MockDoRolesMatch(true, It.IsAny<Account>(), childPasswordGroup);

        await Assert.ThrowsAsync<AccountNotFoundException>(async () => await Sut.MoveChildPasswordGroup(childPasswordGroup.Id, newParentPasswordGroup.Id, userId));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }

    [Fact]
    public async Task ShouldThrow_WhenParentPasswordGroupDoesNotExist()
    {
        var newParentPasswordGroupId = Guid.NewGuid();
        var childPasswordGroup = Utils.GetValidChildPasswordGroup();
        var user = Utils.GetValidUser();

        var passwords = new List<Password>();

        MockGetAccount(user, user.Id);

        MockGetPasswordGroup(null, newParentPasswordGroupId);

        MockGetValidPasswordGroup(childPasswordGroup, childPasswordGroup.Id);

        MockDoRolesMatch(true, user, It.IsAny<PasswordGroup>());

        MockDoRolesMatch(true, user, childPasswordGroup);

        await Assert.ThrowsAsync<ParentPasswordGroupNotFoundException>(async () => await Sut.MoveChildPasswordGroup(childPasswordGroup.Id, newParentPasswordGroupId, user.Id));
        PasswordGroupRepoMock.Verify(x => x.UpdateOneAsync(It.IsAny<PasswordGroup>()), Times.Never);
    }
}
