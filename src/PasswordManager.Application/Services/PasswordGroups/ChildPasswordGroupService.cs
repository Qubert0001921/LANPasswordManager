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

public class ChildPasswordGroupService : IChildPasswordGroupService
{
    private readonly IPasswordGroupRepository _passwordGroupRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordRepository _passwordRepository;
    private readonly IPasswordGroupHelperService _passwordGroupHelper;
    private readonly IPasswordGroupService _passwordGroupService;
    private readonly IAccountHelperService _accountHelperService;
    private readonly IMapper _mapper;

    public ChildPasswordGroupService(
        IPasswordGroupRepository passwordGroupRepository, 
        IAccountRepository accountRepository,
        IPasswordRepository passwordRepository,
        IPasswordGroupHelperService passwordGroupHelperService,
        IPasswordGroupService passwordGroupService,
        IAccountHelperService accountHelperService,
        IMapper mapper)
    {
        _passwordGroupRepository = passwordGroupRepository;
        _accountRepository = accountRepository;
        _passwordRepository = passwordRepository;
        _passwordGroupHelper = passwordGroupHelperService;
        _passwordGroupService = passwordGroupService;
        _accountHelperService = accountHelperService;
        _mapper = mapper;
    }

    public async Task CreateChildPasswordGroup(PasswordGroupDto dto, AccountDto creator)
    {
        var parentPasswordGroup = await _passwordGroupRepository.GetByIdAsync(dto.ParentPasswordGroupId);

        if(parentPasswordGroup is null)
        {
            throw new Exception("Assigned parent password group doesn't exist");
        }

        var account = await _accountRepository.GetByIdAsync(creator.Id);
        if(account is null)
        {
            throw new Exception("Account doesn't exist");
        }

        if(!account.IsAdmin)
        {
            var hasAccessRole = await _passwordGroupHelper.HasAccountPasswordGroupRole(account, parentPasswordGroup); 

            if(!hasAccessRole)
            {
                throw new Exception("Cannot create child password group because user account doesn't have appropriate role");
            }
        }      

        var passwords = _mapper.Map<List<Password>>(dto.Passwords);
        var childPasswordGroup = PasswordGroup.CreateChildPasswordGroup(
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

    public async Task MoveChildPasswordGroup(PasswordGroupDto childDto, PasswordGroupDto parentDto, AccountDto accountDto)
    {
        var account = await _accountHelperService.CheckIfAccountExists(accountDto.Id);

        var existingChild = await _passwordGroupRepository.GetChildPasswordGroupByIdAsync(childDto.Id);

        if(existingChild is null)
        {
            throw new PasswordGroupNotFoundException();
        }

        var existingParent = await _passwordGroupRepository.GetByIdAsync(parentDto.Id);

        if(existingParent is null)
        {
            throw new ParentPasswordGroupNotFoundException();
        }

        if(!account.IsAdmin)
        {
            await _passwordGroupHelper.CheckAccountAndPasswordGroupRoles(account, existingParent);
        }

        existingChild.MovePasswordGroup(existingParent);

        await _passwordGroupRepository.UpdateOneAsync(existingChild);
    }

    public async Task RemoveChildPasswordGroup(PasswordGroupDto dto)
    {
        var childPasswordGroup = await _passwordGroupRepository.GetChildPasswordGroupByIdAsync(dto.Id);
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
}
