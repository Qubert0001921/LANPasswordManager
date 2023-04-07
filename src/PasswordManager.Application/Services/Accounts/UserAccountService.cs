using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.Accounts;

public class UserAccountService : IUserAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public UserAccountService(
        IAccountRepository accountRepository,
        IMapper mapper
    )
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task CreateUserAccount(AccountDto dto, AccountDto creator)
    {
        var allAdminAccounts = await _accountRepository.GetAllAdminAccountsAsync();
        if(!allAdminAccounts.Any())
        {
            throw new Exception("Cannot create user account because there is no admin account");
        }

        var existingCreator = await _accountRepository.GetByIdAsync(creator.Id);
        if(existingCreator is null)
        {
            throw new Exception("Creator account doesn't exist");
        }

        if(!existingCreator.IsAdmin)
        {
            throw new Exception("Creator account must be an admin account");
        }

        var existingAccount = await _accountRepository.GetAccountByLoginAsync(dto.Login);
        if(existingAccount is not null)
        {
            throw new Exception("Account login id already taken");
        }

        var userRoles = _mapper.Map<IEnumerable<Role>>(dto.Roles);
        var user = Account.CreateUserAccount(
            dto.Id,
            dto.Login,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Password,
            userRoles
        );

        await _accountRepository.CreateOneAsync(user);
    }
}
