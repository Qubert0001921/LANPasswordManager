using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Moq;

using PasswordManager.Application.Services.PasswordGroups;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Tests.PasswordGroupHelperServiceTests;

public abstract class BasePasswordGroupHelperServiceTests
{
    protected readonly Mock<IAccountRepository> AccountRepoMock = new Mock<IAccountRepository>();
    protected readonly Mock<IPasswordRepository> PasswordRepoMock = new Mock<IPasswordRepository>();
    protected readonly Mock<IPasswordGroupService> PasswordGroupServiceMock = new Mock<IPasswordGroupService>();
    protected readonly Mock<IPasswordGroupRepository> PasswordGroupRepoMock = new Mock<IPasswordGroupRepository>();
    
    protected readonly IPasswordGroupHelperService Sut;

    public BasePasswordGroupHelperServiceTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>())
            .CreateMapper();

        Sut = new PasswordGroupHelperService(
            accountRepository: AccountRepoMock.Object,
            passwordRepository:  PasswordRepoMock.Object,
            mapper: mapper,
            passwordGroupService: PasswordGroupServiceMock.Object,
            passwordGroupRepository: PasswordGroupRepoMock.Object
        );
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
