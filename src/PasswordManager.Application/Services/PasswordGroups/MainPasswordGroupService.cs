using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Application.Extensions;
using PasswordManager.Application.Validators;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class MainPasswordGroupService : IMainPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupHelperService _passwordGroupHelper;

    public MainPasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository,
        IAccountRepository accountRepository, 
        IRoleRepository roleRepository,
        IPasswordRepository passwordRepository,
        IPasswordGroupHelperService passwordGroupHelperService,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _roleRepository = roleRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupHelper = passwordGroupHelperService;
        _passwordGroupRepository = passwordGroupRepository;
        _mapper = mapper;
    }

    public async Task AddAccessRoleToMainPasswordGroup(Guid mainPasswordGroupId, Guid roleId, Guid accountId)
    {
        var account = await CheckAdminAccount(accountId);

        var passwordGroup = await _passwordGroupHelper.GetAndValidPasswordGroup(mainPasswordGroupId, PasswordGroupType.Main);

        var role = await _roleRepository.GetByIdAsync(roleId);
        if(role is null)
        {
            throw new RoleNotFoundException();
        }

        passwordGroup.AddAccessRole(role);

        await _passwordGroupRepository.UpdateOneAsync(passwordGroup);
    }

    public async Task CreateMainPasswordGroup(PasswordGroupDto dto, Guid accountId)
    {
        var existingCreator = await CheckAdminAccount(accountId);

        var validator = new MainPasswordGroupDtoValidator();
        await validator.ValidateAndThrowValidationProcessExceptionAsync(dto);

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var accessRoles = _mapper.Map<List<Role>>(dto.AccessRoles);
        var passwordGroup = PasswordGroup.CreateMainPasswordGroup(dto.Id, dto.Name, passwords, accessRoles, existingCreator);

        await _passwordGroupRepository.CreateOneAsync(passwordGroup);
    }

    public async Task<IEnumerable<PasswordGroupDto>> GetAllMainPasswordGroups()
    {
        var passwordGroups = await _passwordGroupRepository.GetAllMainPasswordGroupsAsync();
        var models = _mapper.Map<IEnumerable<PasswordGroupDto>>(passwordGroups);
        return models;
    }

    public async Task<PasswordGroupDto?> GetMainPasswordGroupById(Guid id)
    {
        var passwordGroup = await _passwordGroupHelper.GetAndValidPasswordGroup(id, PasswordGroupType.Main);
        var model = _mapper.Map<PasswordGroupDto>(passwordGroup);
        return model;
    }

    public async Task RemoveAccessRoleFromMainPasswordGroup(Guid mainPasswordGroupId, Guid roleId, Guid accountId)
    {
        var account = await CheckAdminAccount(accountId);

        var passwordGroup = await _passwordGroupHelper.GetAndValidPasswordGroup(mainPasswordGroupId, PasswordGroupType.Main);

        var role = await _roleRepository.GetByIdAsync(roleId);
        if(role is null)
        {
            throw new RoleNotFoundException();
        }

        passwordGroup.RemoveAccessRole(role);

        await _passwordGroupRepository.UpdateOneAsync(passwordGroup);
    }

    public async Task RemoveMainPasswordGroup(Guid passwordGroupId, Guid accountId)
    {
        var account = await CheckAdminAccount(accountId);

        var passwordGroup = await _passwordGroupHelper.GetAndValidPasswordGroup(passwordGroupId, PasswordGroupType.Main);

        var allChildren = await _passwordGroupHelper.GetAllChildrenOfPasswordGroup(passwordGroup);
        var allChildrenIds = allChildren.Select(child => child.Id);
        var allPasswordIds = _passwordGroupHelper.GetAllPasswordIdsOfPasswordGroup(passwordGroup, allChildren);

        // Removing all passwords
        await _passwordRepository.RemoveRangeByIdsAsync(allPasswordIds);

        // Removing all children of the password group
        await _passwordGroupRepository.RemoveRangeByIdsAsync(allChildrenIds);

        // Removing the password group
        await _passwordGroupRepository.RemoveOneByIdAsync(passwordGroup.Id);
    }

    private async Task<Account> CheckAdminAccount(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if(account is null)
        {
            throw new AccountNotFoundException();
        }

        if(!account.IsAdmin)
        {
            throw new AdminPermissionRequiredException();
        }

        return account;
    }
}
