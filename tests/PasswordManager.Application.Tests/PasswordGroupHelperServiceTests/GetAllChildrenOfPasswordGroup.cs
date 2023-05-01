using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Tests.PasswordGroupHelperServiceTests;

public class GetAllChildrenOfPasswordGroup : BasePasswordGroupHelperServiceTests
{
    [Fact]
    public async Task ShouldGetAllChildren_WhenDataIsValid()
    {
        var passwordGroup = Utils.GetValidChildPasswordGroup();
        var children = new List<PasswordGroup>();

        for(int i = 0; i<10; i++)
        {
            children.Add(Utils.GetValidChildPasswordGroup());
        }

        var expectedIds = children.Select(x => x.Id).ToList();

        PasswordGroupRepoMock.Setup(x => x.GetChildrenOfPasswordGroupAsync(passwordGroup))
            .ReturnsAsync(children);

        foreach(var child in children.ToList())
        {   
            var childPasswordGroups = new List<PasswordGroup>();
            for(int i = 0; i<2; i++)
            {
                childPasswordGroups.Add(Utils.GetValidChildPasswordGroup());
            }
            expectedIds.AddRange(childPasswordGroups.Select(x => x.Id));

            PasswordGroupRepoMock.Setup(x => x.GetChildrenOfPasswordGroupAsync(child))
                .ReturnsAsync(childPasswordGroups);
        }

        var actual = await Sut.GetAllChildrenOfPasswordGroup(passwordGroup);
        var actualIds = actual.Select(x => x.Id);

        Assert.Equal(actualIds.Count(), expectedIds.Count());

        foreach (var actualId in actualIds)
        {
            Assert.Contains(actualId, expectedIds);
        }
    }
}
