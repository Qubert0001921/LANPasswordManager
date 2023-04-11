using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Moq;

using PasswordManager.Application.Services.PasswordGroups;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Tests.ChildPasswordGroupServiceTests;

public abstract class BaseChildPasswordGroupServiceTests
{
    protected readonly Mock<IPasswordGroupRepository> PasswordGroupRepoMock = new Mock<IPasswordGroupRepository>();
    protected readonly Mock<IAccountRepository> AccountRepoMock = new Mock<IAccountRepository>();
    protected readonly Mock<IPasswordRepository> PasswordRepoMock = new Mock<IPasswordRepository>();
    protected readonly Mock<IPasswordGroupHelperService> PasswordGroupHelperMock = new Mock<IPasswordGroupHelperService>();
    protected readonly Mock<IPasswordGroupService> PasswordGroupServiceMock = new Mock<IPasswordGroupService>();
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
            mapper: mapper,
            passwordGroupService: PasswordGroupServiceMock.Object
        );
    }
}
