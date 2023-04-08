using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IChildPasswordGroupService
{
    Task CreateChildPasswordGroup(PasswordGroupDto dto, AccountDto creator);
    Task MoveChildPasswordGroup(PasswordGroupDto childDto, PasswordGroupDto parentDto, AccountDto accountDto);
    Task RemoveChildPasswordGroup(PasswordGroupDto dto);
    Task<PasswordGroupDto> GetChildPasswordGroupById(Guid id);
    Task<IEnumerable<PasswordGroupDto>> GetAllChildPasswordGroups();
}
