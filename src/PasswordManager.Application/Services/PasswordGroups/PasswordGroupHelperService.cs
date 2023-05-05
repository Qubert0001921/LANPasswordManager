using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;
using PasswordManager.Domain.Exceptions;
using PasswordManager.Domain.Repositories;

namespace PasswordManager.Application.Services.PasswordGroups;

public class PasswordGroupHelperService : IPasswordGroupHelperService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IPasswordGroupService _passwordGroupService;
    private readonly IMapper _mapper;

    public PasswordGroupHelperService(
        IAccountRepository accountRepository,
        IPasswordRepository passwordRepository,
        IPasswordGroupRepository passwordGroupRepository,
        IPasswordGroupService passwordGroupService,
        IMapper mapper
    )
    {
        _accountRepository = accountRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupRepository = passwordGroupRepository;
        _passwordGroupService = passwordGroupService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PasswordGroup>> GetAllChildrenOfPasswordGroup(PasswordGroup passwordGroup)
    {
        var allChildren = (await _passwordGroupRepository.GetChildrenOfPasswordGroupAsync(passwordGroup)).ToList();

        foreach (var child in allChildren.ToList())
        {
            var childrenOfChild = await GetAllChildrenOfPasswordGroup(child);
            allChildren.AddRange(childrenOfChild);
        }

        return allChildren;
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

    public async Task<PasswordGroup> GetAndValidPasswordGroup(Guid passwordGroupId, PasswordGroupType passwordGroupType)
    {
        var passwordGroup = await _passwordGroupRepository.GetByIdAsync(passwordGroupId);
        if(passwordGroup is null)
        {
            throw new PasswordGroupNotFoundException();
        }

        if(passwordGroup.PasswordGroupType != passwordGroupType)
        {
            throw new PasswordGroupTypeInvalidException();
        }

        return passwordGroup;
    }

    public async Task<bool> HasAccountPasswordGroupRole(Account account, PasswordGroup passwordGroup)
    {
        var accessRoleModels = await _passwordGroupService.GetAccessRolesByPasswordGroupId(passwordGroup.Id);
        var accessRoles = _mapper.Map<List<Role>>(accessRoleModels);
        var hasAccessRole = false;

        foreach (var role in account.Roles)
        {
            if(accessRoles.Any(x => x.Id == role.Id))
            {
                hasAccessRole = true;
                break;
            }
        }  

        return hasAccessRole;
    }
}
