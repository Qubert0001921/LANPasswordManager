using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services;

public class ChildPasswordGroupService : IChildPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IMapper _mapper;

    public ChildPasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _mapper = mapper;
    }

    public async Task CreateChildPasswordGroup(PasswordGroupDto dto, AccountDto creator)
    {
        var parentPasswordGroup = await _passwordGroupRepository.GetById(dto.ParentPasswordGroupId);

        if(parentPasswordGroup is null)
        {
            throw new Exception("Assigned parent password group doesn't exist");
        }

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var childPasswordGroup = PasswordGroup.CreateChildPasswordGroup(
            dto.Name,
            passwords,
            parentPasswordGroup
        );

        await _passwordGroupRepository.CreateOne(childPasswordGroup);
    }

    public async Task MoveChildPasswordGroup(PasswordGroupDto childDto, PasswordGroupDto parentDto)
    {
        var existingChild = await _passwordGroupRepository.GetById(childDto.Id);

        if(existingChild is null)
        {
            throw new Exception("Child password group doesn't exist");
        }

        var existingParent = await _passwordGroupRepository.GetById(parentDto.Id);

        if(existingParent is null)
        {
            throw new Exception("Parent password group doesn't exist");
        }

        existingChild.MovePasswordGroup(existingParent);

        await _passwordGroupRepository.UpdateOne(existingChild);
    }

    public Task RemoveChildPasswordGroup(PasswordGroupDto dto, AccountDto account)
    {
        throw new NotImplementedException();
    }
}
