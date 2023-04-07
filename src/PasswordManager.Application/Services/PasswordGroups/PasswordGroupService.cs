using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class PasswordGroupService : IPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupHelperService _passwordGroupHelper;
    private readonly IMapper _mapper;

    public PasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IPasswordRepository passwordRepository,
        IPasswordGroupHelperService passwordGroupHelperService,
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupHelper = passwordGroupHelperService;
        _mapper = mapper;
    }

    public async Task AddPasswordToPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(dto.Id);
        var password = new Password(
            passwordDto.Id, 
            passwordDto.PasswordCipher
        );

        passwordGroup.AddPassword(password);

        await _passwordRepository.CreateOneAsync(password);

        await _passwordGroupRepository.AddPasswordToPasswordGroupAsync(passwordGroup, password);
    }

    public async Task RemovePasswordFromPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(dto.Id);
        var password = await _passwordGroupHelper.CheckIfPasswordExists(passwordDto.Id);

        passwordGroup.RemovePassword(password);
        
        await _passwordGroupRepository.RemovePasswordFromPasswordGroupAsync(passwordGroup, password);
        await _passwordRepository.RemoveOneByIdAsync(password.Id);
    }

    public async Task<List<RoleDto>> GetAccessRolesByPasswordGroupId(Guid id)
    {
        var passwordGroup = await _passwordGroupRepository.GetByIdAsync(id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

        if(passwordGroup.PasswordGroupType == PasswordGroupType.Main)
        {
            var roles = _mapper.Map<List<RoleDto>>(passwordGroup.AccessRoles);
            return roles;
        }

        if(passwordGroup.ParentPasswordGroup is null)
        {
            throw new Exception("Parent password group doesn't exist");
        }

        var result = await GetAccessRolesByPasswordGroupId(passwordGroup.ParentPasswordGroup.Id);
        return result;
    }


    public async Task<IEnumerable<PasswordDto>> GetPasswordsOfPasswordGroup(PasswordGroupDto passwordGroupDto, AccountDto accountDto)
    {
        var account = await _passwordGroupHelper.CheckIfAccountExists(accountDto.Id);
        _passwordGroupHelper.ThrowIfNotAdmin(account, "Admin account musn't get passwords of password group");

        var passwordGroup = await _passwordGroupRepository.GetByIdAsync(passwordGroupDto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }

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
            throw new Exception("Password group doesn't exist");
        }
        return passwordGroup;
    }
}
