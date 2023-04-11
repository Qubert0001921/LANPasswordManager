using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Application.Extensions;
using PasswordManager.Application.Services.Accounts;
using PasswordManager.Application.Validators;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class ChildPasswordGroupService : IChildPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupHelperService _passwordGroupHelper;
    private readonly IPasswordGroupService _passwordGroupService;
    private readonly IMapper _mapper;

    public ChildPasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IAccountRepository accountRepository,
        IPasswordRepository passwordRepository,
        IPasswordGroupHelperService passwordGroupHelperService,
        IPasswordGroupService passwordGroupService,
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _accountRepository = accountRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupHelper = passwordGroupHelperService;
        _passwordGroupService = passwordGroupService;
        _mapper = mapper;
    }

    public async Task CreateChildPasswordGroup(PasswordGroupDto dto, Guid accountId)
    {
        var validator = new PasswordGroupDtoValidator();
        await validator.ValidateAndThrowValidationProcessExceptionAsync(dto);

        var parentPasswordGroup = await CheckIfParentPasswordGroupExists(dto.ParentPasswordGroupId);

        var account = await CheckIfAccountExists(accountId);
  
        await CheckIfNotAdminAndThrowIfRolesDoesNotMatch(account, parentPasswordGroup); 

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var childPasswordGroup = PasswordGroup.CreateChildPasswordGroup(
            dto.Id,
            dto.Name,
            passwords,
            parentPasswordGroup
        );

        await _passwordGroupRepository.CreateOneAsync(childPasswordGroup);
    }


    // Is it necessary??
    public async Task<IEnumerable<PasswordGroupDto>> GetAllChildPasswordGroups()
    {
        var passwordGroups = await _passwordGroupRepository.GetAllChildPasswordGroupsAsync();
        var models = _mapper.Map<IEnumerable<PasswordGroupDto>>(passwordGroups);
        return models;
    }

    // Is it necessary??
    public async Task<PasswordGroupDto> GetChildPasswordGroupById(Guid id)
    {
        var passwordGroup = await _passwordGroupRepository.GetChildPasswordGroupByIdAsync(id);
        var model = _mapper.Map<PasswordGroupDto>(passwordGroup);
        return model;
    }

    public async Task MoveChildPasswordGroup(Guid childPasswordGroupId, Guid newParentPasswordGroupId, Guid accountId)
    {
        var account = await CheckIfAccountExists(accountId);

        var existingChild = await _passwordGroupRepository.GetChildPasswordGroupByIdAsync(childPasswordGroupId);

        if(existingChild is null)
        {
            throw new PasswordGroupNotFoundException();
        }

        var existingParent = await CheckIfParentPasswordGroupExists(newParentPasswordGroupId);

        await CheckIfNotAdminAndThrowIfRolesDoesNotMatch(account, existingParent);

        existingChild.MovePasswordGroup(existingParent);

        await _passwordGroupRepository.UpdateOneAsync(existingChild);
    }

    public async Task RemoveChildPasswordGroup(Guid passwordGroupId)
    {
        var childPasswordGroup = await _passwordGroupRepository.GetChildPasswordGroupByIdAsync(passwordGroupId);
        if(childPasswordGroup is null)
        {
            throw new PasswordGroupNotFoundException();
        }

        var allChildren = await _passwordGroupHelper.GetAllChildrenOfPasswordGroup(childPasswordGroup);
        var allChildrenIds = allChildren.Select(child => child.Id);
        var allPasswordIds = _passwordGroupHelper.GetAllPasswordIdsOfPasswordGroup(childPasswordGroup, allChildren);

        // Removing all passwords
        await _passwordRepository.RemoveRangeByIdsAsync(allPasswordIds);

        // Removing all children of the password group
        await _passwordGroupRepository.RemoveRangeByIdsAsync(allChildrenIds);

        // Removing the password group
        await _passwordGroupRepository.RemoveOneByIdAsync(childPasswordGroup.Id);

    }

    private async Task CheckIfNotAdminAndThrowIfRolesDoesNotMatch(Account account, PasswordGroup passwordGroup)
    {
        if(!account.IsAdmin)
        {
            var hasAccessRole = await _passwordGroupHelper.HasAccountPasswordGroupRole(account, passwordGroup); 

            if(!hasAccessRole)
            {
                throw new AccountPasswordGroupRolesInconsistencyException();
            }
        } 
    }

    private async Task<Account> CheckIfAccountExists(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if(account is null)
        {
            throw new AccountNotFoundException();
        }

        return account;
    }

    private async Task<PasswordGroup> CheckIfParentPasswordGroupExists(Guid parentPasswordGroupId)
    {
        var parent = await _passwordGroupRepository.GetByIdAsync(parentPasswordGroupId);

        if(parent is null)
        {
            throw new ParentPasswordGroupNotFoundException();
        }

        return parent;
    }
}
