using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services;

public interface IPasswordGroupService
{
    Task AddPasswordToPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto);
    Task RemovePasswordFromPasswordGroup(PasswordGroupDto dto, PasswordDto passwordDto);
    Task<List<RoleDto>> GetAccessRolesByPasswordGroupId(Guid id);
}
