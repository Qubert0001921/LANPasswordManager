using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class PasswordGroupHelperService : IPasswordGroupHelperService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupRepository _passwordGroupRepository;

    public PasswordGroupHelperService(
        IAccountRepository accountRepository,
        IPasswordRepository passwordRepository,
        IPasswordGroupRepository passwordGroupRepository
    )
    {
        _accountRepository = accountRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupRepository = passwordGroupRepository;
    }

    public async Task<Account> CheckIfAccountExists(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if(account is null)
        {
            throw new Exception("Account doesn't exist");
        }

        return account;
    }

    public async Task<IEnumerable<Guid>> GetAllChildrenIdsOfPasswordGroup(PasswordGroup passwordGroup)
    {
        var allChildrenPasswordGroup = await GetAllChildrenOfPasswordGroup(passwordGroup);
        var allChildrenPasswordGroupIds = allChildrenPasswordGroup.Select(childPasswordGroup => childPasswordGroup.Id);
        return allChildrenPasswordGroupIds;
    }

    public async Task<IEnumerable<PasswordGroup>> GetAllChildrenOfPasswordGroup(PasswordGroup passwordGroup)
    {
        var allChildren = (await _passwordGroupRepository.GetChildrenOfPasswordGroupAsync(passwordGroup)).ToList();

        foreach (var child in allChildren)
        {
            var childrenOfChild = await GetAllChildrenOfPasswordGroup(child);
            allChildren.AddRange(childrenOfChild);
        }

        return allChildren;
    }

    public async Task<IEnumerable<Guid>> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup)
    {
        var allChildrenPasswordGroups = (await GetAllChildrenOfPasswordGroup(passwordGroup)).ToList();
        return GetAllPasswordIdsOfPasswordGroup(passwordGroup, allChildrenPasswordGroups);
    }

    public IEnumerable<Guid> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup, IEnumerable<PasswordGroup> childrenPasswordGroups)
    {
        var allChildrenPasswordIds = passwordGroup.Passwords
            .Select(password => password.Id)
            .ToList();

        var allChildrenPasswordGroups = childrenPasswordGroups;

        foreach (var child in allChildrenPasswordGroups)
        {
            var childPasswordIds = child.Passwords.Select(x => x.Id);
            allChildrenPasswordIds.AddRange(childPasswordIds);
        }

        return allChildrenPasswordIds;
    }

    public void ThrowIfNotAdmin(Account account, string message)
    {
        if(!account.IsAdmin)
        {
            throw new Exception(message);
        }
    }

    public async Task<Password> CheckIfPasswordExists(Guid id)
    {
        var password = await _passwordRepository.GetByIdAsync(id);
        if(password is null)
        {
            throw new Exception("Password doesn't exist");
        }
        return password;
    }
}
