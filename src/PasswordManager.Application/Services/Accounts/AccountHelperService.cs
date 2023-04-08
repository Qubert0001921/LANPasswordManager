using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.Accounts;

public class AccountHelperService : IAccountHelperService
{
    private readonly IAccountRepository _accountRepository;

    public AccountHelperService(
        IAccountRepository accountRepository
    )
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> CheckIfAccountExists(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if(account is null)
        {
            throw new AccountNotFoundException();
        }

        return account;
    }
}
