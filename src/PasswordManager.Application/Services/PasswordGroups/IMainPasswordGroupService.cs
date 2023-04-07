using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IMainPasswordGroupService
{
    Task CreateMainPasswordGroup(PasswordGroupDto dto, AccountDto creator);
    Task RemoveMainPasswordGroup(PasswordGroupDto dto, AccountDto account);
    Task AddAccessRoleToMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto);
    Task RemoveAccessRoleFromMainPasswordGroup(PasswordGroupDto mainDto, RoleDto roleDto, AccountDto accountDto);
    Task<PasswordGroupDto?> GetMainPasswordGroupById(Guid id);
    Task<IEnumerable<PasswordGroupDto>> GetAllMainPasswordGroups();
}
