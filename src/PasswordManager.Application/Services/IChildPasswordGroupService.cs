using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services;

public interface IChildPasswordGroupService
{
    Task CreateChildPasswordGroup(PasswordGroupDto dto, AccountDto creator);
    Task MoveChildPasswordGroup(PasswordGroupDto childDto, PasswordGroupDto parentDto);
    Task RemoveChildPasswordGroup(PasswordGroupDto dto, AccountDto account);
}
