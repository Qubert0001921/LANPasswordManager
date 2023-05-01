using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Moq;

using PasswordManager.Application.Services.PasswordGroups;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Tests.ChildPasswordGroupServiceTests;

public abstract class BaseChildPasswordGroupServiceTests
{
    protected readonly Mock<IPasswordGroupRepository> PasswordGroupRepoMock = new Mock<IPasswordGroupRepository>();
    protected readonly Mock<IAccountRepository> AccountRepoMock = new Mock<IAccountRepository>();
    protected readonly Mock<IPasswordRepository> PasswordRepoMock = new Mock<IPasswordRepository>();
    protected readonly Mock<IPasswordGroupHelperService> PasswordGroupHelperMock = new Mock<IPasswordGroupHelperService>();
    protected readonly IChildPasswordGroupService Sut;

    public BaseChildPasswordGroupServiceTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>())
            .CreateMapper();

        Sut = new ChildPasswordGroupService(
            passwordGroupRepository: PasswordGroupRepoMock.Object,
            accountRepository: AccountRepoMock.Object,
            passwordRepository:  PasswordRepoMock.Object,
            passwordGroupHelperService: PasswordGroupHelperMock.Object,
            mapper: mapper
        );
    }

    protected void MockDoRolesMatch(bool match, Account account, PasswordGroup passwordGroup)
    {
        PasswordGroupHelperMock.Setup(x => x.HasAccountPasswordGroupRole(account, passwordGroup))
            .ReturnsAsync(match);
    }

    protected void MockGetValidPasswordGroup(PasswordGroup passwordGroup, Guid id)
    {
        PasswordGroupHelperMock.Setup(x => x.GetAndValidPasswordGroup(id, PasswordGroupType.Child))
            .ReturnsAsync(passwordGroup);
    }

    protected void MockGetAccount(Account? returns, Guid id)
    {
        AccountRepoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(returns);
    }

    protected void MockGetPasswordGroup(PasswordGroup? returns, Guid id)
    {
        PasswordGroupRepoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(returns);
    }
}
