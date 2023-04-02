using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services;

public class PasswordGroupService : IPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IMapper _mapper;

    public PasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IPasswordRepository passwordRepository,
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _passwordRepository = passwordRepository;
        _mapper = mapper;
    }

    public async Task AddPasswordToPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(dto);
        var password = new Password(
            passwordDto.Id, 
            passwordDto.PasswordCipher
        );

        passwordGroup.AddPassword(password);

        await _passwordRepository.CreateOne(password);

        await _passwordGroupRepository.AddPasswordToPasswordGroup(passwordGroup, password);
    }

    public async Task RemovePasswordFromPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto)
    {
        var passwordGroup = await CheckIfPasswordGroupExists(dto);
        var password = await CheckIfPasswordExists(passwordDto);

        passwordGroup.RemovePassword(password);
        
        await _passwordGroupRepository.RemovePasswordFromPasswordGroup(passwordGroup, password);
        await _passwordRepository.RemoveOneById(password.Id);
    }

    public async Task<List<RoleDto>> GetAccessRolesByPasswordGroupId(Guid id)
    {
        var passwordGroup = await _passwordGroupRepository.GetById(id);
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

    private async Task<PasswordGroup> CheckIfPasswordGroupExists(PasswordGroupDto dto)
    {
        if(dto is null)
        {
            throw new ArgumentNullException("Password group is null");
        }

        var passwordGroup = await _passwordGroupRepository.GetById(dto.Id);
        if(passwordGroup is null)
        {
            throw new Exception("Password group doesn't exist");
        }
        return passwordGroup;
    }

    private async Task<Password> CheckIfPasswordExists(PasswordDto dto)
    {
        if(dto is null)
        {
            throw new ArgumentNullException("Password is null");
        }

        var password = await _passwordRepository.GetById(dto.Id);
        if(password is null)
        {
            throw new Exception("Password doesn't exist");
        }
        return password;
    }
}
