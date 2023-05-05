using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Moq;
using PasswordManager.Application.Dtos;
using PasswordManager.Application.Services.PasswordGroups;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Tests.PasswordGroupServiceTests;

public abstract class BasePasswordGroupServiceTests
{
    protected readonly Mock<IPasswordGroupRepository> PasswordGroupRepoMock = new Mock<IPasswordGroupRepository>();
    protected readonly Mock<IAccountRepository> AccountRepoMock = new Mock<IAccountRepository>();
    protected readonly Mock<IRoleRepository> RoleRepoMock = new Mock<IRoleRepository>();
    protected readonly Mock<IPasswordRepository> PasswordRepoMock = new Mock<IPasswordRepository>();
    protected readonly Mock<IPasswordGroupHelperService> PasswordGroupHelperMock = new Mock<IPasswordGroupHelperService>();
    protected readonly IPasswordGroupService Sut;

    public BasePasswordGroupServiceTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>())
            .CreateMapper();

        Sut = new PasswordGroupService(
            passwordGroupRepository: PasswordGroupRepoMock.Object,
            accountRepository: AccountRepoMock.Object,
            passwordRepository:  PasswordRepoMock.Object,
            passwordGroupHelperService: PasswordGroupHelperMock.Object,
            mapper: mapper
        );
    }

    protected PasswordGroupDto GetPasswordGroupDto(PasswordGroup passwordGroup) => new PasswordGroupDto()
    {
        Id = passwordGroup.Id,
        AccessRoles = new List<RoleDto>(),
        Name = passwordGroup.Name,
        ParentPasswordGroupId = passwordGroup.ParentPasswordGroup.Id,
        Passwords = new List<PasswordDto>()
    };

    protected AccountDto GetAccontDto(Account account) =>new AccountDto()
    {
        Id = account.Id,
        Email = account.Email,
        FirstName = account.FirstName,
        IsAdmin = account.IsAdmin,
        LastName = account.LastName,
        Login = account.Login,
        Password = account.Password,
        Roles =  new List<RoleDto>()
    };

    protected PasswordDto GetPasswordDto() => new PasswordDto()
    {
        Id = Guid.NewGuid(),
        PasswordCipher= "feFEFIUWEHBFGKUWYEGK"
    };

    protected void MockGetAccount(Account? returns, Guid id)
    {
        AccountRepoMock.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(returns);
    }
}
