using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
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

    public async Task AddAccessRoleToMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto)
    {
        var account = await CheckIfAdminAccountExists(accountDto);

        var passwordGroup = await _passwordGroupRepository.GetMainPasswordGroupByIdAsync(mainDto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

        var role = await _roleRepository.GetByIdAsync(roleDto.Id);
        if(role is null)
        {
            throw new Exception("Role doesn't exist");
        }

        passwordGroup.AddAccessRole(role);

        await _passwordGroupRepository.UpdateOneAsync(passwordGroup);
    }

    public async Task CreateMainPasswordGroup(PasswordGroupDto dto, AccountDto creator)
    {
        var existingCreator = await CheckIfAdminAccountExists(creator);

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var accessRoles = _mapper.Map<List<Role>>(dto.AccessRoles);
        var passwordGroup = PasswordGroup.CreateMainPasswordGroup(dto.Name, passwords, accessRoles, existingCreator);

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
        var passwordGroup = await _passwordGroupRepository.GetMainPasswordGroupByIdAsync(id);
        var model = _mapper.Map<PasswordGroupDto>(passwordGroup);
        return model;
    }

    public async Task RemoveAccessRoleFromMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto)
    {
        var account = await CheckIfAdminAccountExists(accountDto);

        var passwordGroup = await _passwordGroupRepository.GetMainPasswordGroupByIdAsync(mainDto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

        var role = await _roleRepository.GetByIdAsync(roleDto.Id);
        if(role is null)
        {
            throw new Exception("Role doesn't exist");
        }

        passwordGroup.RemoveAccessRole(role);

        await _passwordGroupRepository.UpdateOneAsync(passwordGroup);
    }

    public async Task RemoveMainPasswordGroup(PasswordGroupDto dto, AccountDto accountDto)
    {
        var account = await CheckIfAdminAccountExists(accountDto);

        var passwordGroup = await _passwordGroupRepository.GetMainPasswordGroupByIdAsync(dto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("password group doesn't exist");
        }

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

    private async Task<Account> CheckIfAdminAccountExists(AccountDto accountDto)
    {
        var account = await _accountRepository.GetByIdAsync(accountDto.Id);
        if(account is null)
        {
            throw new Exception("Account doesn't exist");
        }

        if(!account.IsAdmin)
        {
            throw new Exception("Account must be an admin account");
        }

        return account;
    }
}
