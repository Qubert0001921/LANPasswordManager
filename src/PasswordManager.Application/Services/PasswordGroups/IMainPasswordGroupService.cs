using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IMainPasswordGroupService
{
    Task CreateMainPasswordGroup(PasswordGroupDto dto, Guid accountId);
    Task RemoveMainPasswordGroup(Guid passwordGroupId, Guid accountId);
    Task AddAccessRoleToMainPasswordGroup(Guid mainPasswordGroupId, Guid roleId, Guid accountId);
    Task RemoveAccessRoleFromMainPasswordGroup(Guid mainPasswordGroupId, Guid roleId, Guid accountId);
    Task<PasswordGroupDto?> GetMainPasswordGroupById(Guid id);
    Task<IEnumerable<PasswordGroupDto>> GetAllMainPasswordGroups();
}
