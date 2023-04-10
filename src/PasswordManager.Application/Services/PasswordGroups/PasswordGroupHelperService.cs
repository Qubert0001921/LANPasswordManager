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

    public async Task<bool> HasAccountPasswordGroupRole(Account account, PasswordGroup passwordGroup)
    {
        var accessRoleModels = await _passwordGroupService.GetAccessRolesByPasswordGroupId(passwordGroup.Id);
        var accessRoles = _mapper.Map<List<Role>>(accessRoleModels);
        var hasAccessRole = false;

        foreach (var role in account.Roles)
        {
            if(accessRoles.Contains(role))
            {
                hasAccessRole = true;
                break;
            }
        }  

        return hasAccessRole;
    }
}
