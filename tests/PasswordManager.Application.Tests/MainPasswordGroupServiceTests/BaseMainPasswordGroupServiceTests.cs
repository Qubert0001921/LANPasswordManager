using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Moq;

using PasswordManager.Application.Services.PasswordGroups;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Tests.MainPasswordGroupServiceTests;

public abstract class BaseMainPasswordGroupServiceTests
{
    protected readonly Mock<IPasswordGroupRepository> PasswordGroupRepoMock = new Mock<IPasswordGroupRepository>();
    protected readonly Mock<IAccountRepository> AccountRepoMock = new Mock<IAccountRepository>();
    protected readonly Mock<IRoleRepository> RoleRepoMock = new Mock<IRoleRepository>();
    protected readonly Mock<IPasswordRepository> PasswordRepoMock = new Mock<IPasswordRepository>();
    protected readonly Mock<IPasswordGroupHelperService> PasswordGroupHelperMock = new Mock<IPasswordGroupHelperService>();
    protected readonly IMainPasswordGroupService Sut;

    public BaseMainPasswordGroupServiceTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>())
            .CreateMapper();

        Sut = new MainPasswordGroupService(
            passwordGroupRepository: PasswordGroupRepoMock.Object,
            accountRepository: AccountRepoMock.Object,
            roleRepository:  RoleRepoMock.Object,
            passwordRepository:  PasswordRepoMock.Object,
            passwordGroupHelperService: PasswordGroupHelperMock.Object,
            mapper: mapper
        );
    }
}
