using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.Accounts;

public class AdminAccountService : IAdminAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AdminAccountService(
        IAccountRepository accountRepository
    )
    {
        _accountRepository = accountRepository;
    }

    public async Task CreateAdminAccount(AccountDto dto, AccountDto creator)
    {
        var allAdminAccounts = await _accountRepository.GetAllAdminAccountsAsync();
        if(allAdminAccounts.Any())
        {
            var existingCreator = await _accountRepository.GetByIdAsync(creator.Id);
            if(existingCreator is null)
            {
                throw new Exception("Creator account doesn't exist");
            }
            
            if(!existingCreator.IsAdmin)
            {
                throw new Exception("Creator account must be an admin account");
            }

            var existingAdmin = await _accountRepository.GetAccountByLoginAsync(dto.Login);
            if(existingAdmin is not null)
            {
                throw new Exception("Login is already taken");
            }
        }

        var admin = Account.CreateAdminAccount(
            dto.Id,
            dto.Login,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Password
        );
        
        await _accountRepository.CreateOneAsync(admin); 
    }
}
