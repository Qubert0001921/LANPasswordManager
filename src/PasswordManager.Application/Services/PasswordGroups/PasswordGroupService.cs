using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Application.Services.Accounts;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class PasswordGroupService : IPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupHelperService _passwordGroupHelper;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public PasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IPasswordRepository passwordRepository,
        IPasswordGroupHelperService passwordGroupHelperService,
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupHelper = passwordGroupHelperService;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task AddPasswordToPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExistsAndThrowIfAdmin(accountDto.Id);

        var passwordGroup = await CheckIfPasswordGroupExists(dto.Id);

        await ThrowIfAccountHasNotPasswordGroupRoles(account, passwordGroup);

        var password = new Password(
            passwordDto.Id, 
            passwordDto.PasswordCipher
        );

        passwordGroup.AddPassword(password);

        await _passwordRepository.CreateOneAsync(password);

        await _passwordGroupRepository.AddPasswordToPasswordGroupAsync(passwordGroup, password);
    }

    public async Task RemovePasswordFromPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExistsAndThrowIfAdmin(accountDto.Id);

        var passwordGroup = await CheckIfPasswordGroupExists(dto.Id);

        await ThrowIfAccountHasNotPasswordGroupRoles(account, passwordGroup);

        var password = await _passwordRepository.GetByIdAsync(passwordDto.Id);
        if(password is null)
        {
            throw new PasswordNotFoundException();
        }

        passwordGroup.RemovePassword(password);
        
        await _passwordGroupRepository.RemovePasswordFromPasswordGroupAsync(passwordGroup, password);
        await _passwordRepository.RemoveOneByIdAsync(password.Id);
    }

    public async Task<List<RoleDto>> GetAccessRolesByPasswordGroupId(Guid id)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(id);

        if(passwordGroup.PasswordGroupType == PasswordGroupType.Main)
        {
            var roles = _mapper.Map<List<RoleDto>>(passwordGroup.AccessRoles);
            return roles;
        }

        if(passwordGroup.ParentPasswordGroup is null)
        {
            throw new ParentPasswordGroupNotFoundException();
        }

        var result = await GetAccessRolesByPasswordGroupId(passwordGroup.ParentPasswordGroup.Id);
        return result;
    }


    public async Task<IEnumerable<PasswordDto>> GetPasswordsOfPasswordGroup(PasswordGroupDto passwordGroupDto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExistsAndThrowIfAdmin(accountDto.Id);

        var passwordGroup = await CheckIfPasswordGroupExists(passwordGroupDto.Id);

        await ThrowIfAccountHasNotPasswordGroupRoles(account, passwordGroup);

        var passwordDtos = _mapper.Map<IEnumerable<PasswordDto>>(passwordGroup.Passwords);
        return passwordDtos;
    }

    public async Task<IEnumerable<PasswordGroupDto>> GetChildrenOfPasswordGroup(PasswordGroupDto dto)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(dto.Id);
        var children = await _passwordGroupRepository.GetChildrenOfPasswordGroupAsync(passwordGroup);
        var childrenModels = _mapper.Map<IEnumerable<PasswordGroupDto>>(children);
        
        return childrenModels;
    }
    
    private async Task<PasswordGroup> CheckIfPasswordGroupExists(Guid id)
    {
        var passwordGroup = await _passwordGroupRepository.GetByIdAsync(id);
        if(passwordGroup is null)
        {
            throw new PasswordGroupNotFoundException();
        }
        return passwordGroup;
    }

    private async Task<Account> CheckIfAccountExistsAndThrowIfAdmin(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if(account is null)
        {
            throw new AccountNotFoundException();
        }
    
        if(account.IsAdmin)
        {
            throw new AdminAccountNotAllowedException();
        }

        return account;
    }

    private async Task ThrowIfAccountHasNotPasswordGroupRoles(Account account, PasswordGroup passwordGroup)
    {
        var hasAccountRoles = await _passwordGroupHelper.HasAccountPasswordGroupRole(account, passwordGroup);
        if(!hasAccountRoles)
        {
            throw new AccountPasswordGroupRolesInconsistencyException();
        }
    }
}
