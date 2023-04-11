using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IChildPasswordGroupService
{
    Task CreateChildPasswordGroup(PasswordGroupDto dto, Guid accountId);
    Task MoveChildPasswordGroup(Guid childPasswordGroupId, Guid newParentPasswordGroupId, Guid accountId);
    Task RemoveChildPasswordGroup(Guid passwordGroupId);
    Task<PasswordGroupDto> GetChildPasswordGroupById(Guid id);
    Task<IEnumerable<PasswordGroupDto>> GetAllChildPasswordGroups();
}
