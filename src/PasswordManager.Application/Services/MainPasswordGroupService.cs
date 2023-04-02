using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services;

public class MainPasswordGroupService : IMainPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;
    private readonly IRoleRepository _roleRepository;

    public MainPasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository,
        IAccountRepository accountRepository, 
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _roleRepository = roleRepository;
        _passwordGroupRepository = passwordGroupRepository;
        _mapper = mapper;
    }

    public async Task AddAccessRoleToMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExists(accountDto);
        ThrowIfNotAdmin(account, "Only admin account can add access role to password group");

        var passwordGroup = await _passwordGroupRepository.GetById(mainDto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

        var role = await _roleRepository.GetById(roleDto.Id);
        if(role is null)
        {
            throw new Exception("Role doesn't exist");
        }

        passwordGroup.AddAccessRole(role);

        await _passwordGroupRepository.UpdateOne(passwordGroup);
    }

    public async Task CreateMainPasswordGroup(PasswordGroupDto dto, AccountDto creator)
    {
        var existingCreator = await CheckIfAccountExists(creator);

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var accessRoles = _mapper.Map<List<Role>>(dto.AccessRoles);
        var passwordGroup = PasswordGroup.CreateMainPasswordGroup(dto.Name, passwords, accessRoles, existingCreator);

        await _passwordGroupRepository.CreateOne(passwordGroup);
    }

    public async Task RemoveAccessRoleFromMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExists(accountDto);
        ThrowIfNotAdmin(account, "Only admin account can remove access role from password group");

        var passwordGroup = await _passwordGroupRepository.GetById(mainDto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

        var role = await _roleRepository.GetById(roleDto.Id);
        if(role is null)
        {
            throw new Exception("Role doesn't exist");
        }

        passwordGroup.RemoveAccessRole(role);

        await _passwordGroupRepository.UpdateOne(passwordGroup);
    }

    public async Task RemoveMainPasswordGroup(PasswordGroupDto dto, AccountDto accountDto)
    {
        var account = await CheckIfAccountExists(accountDto);

        ThrowIfNotAdmin(account, "Only admin account can remove main password group");

        var passwordGroup = await _passwordGroupRepository.GetById(dto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("password group doesn't exist");
        }

        await _passwordGroupRepository.RemoveOneById(dto.Id);
    }

    private async Task<Account> CheckIfAccountExists(AccountDto accnt)
    {
        var account = await _accountRepository.GetById(accnt.Id);
        if(account is null)
        {
            throw new Exception("Account doesn't exist");
        }

        return account;
    }

    private void ThrowIfNotAdmin(Account account, string message)
    {
        if(!account.IsAdmin)
        {
            throw new Exception(message);
        }
    }
}
