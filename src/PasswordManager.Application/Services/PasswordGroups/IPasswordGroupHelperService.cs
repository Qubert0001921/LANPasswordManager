using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IPasswordGroupHelperService
{
    Task<IEnumerable<PasswordGroup>> GetAllChildrenOfPasswordGroup(PasswordGroup passwordGroup);

    IEnumerable<Guid> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup, IEnumerable<PasswordGroup> childrenPasswordGroups);

    Task<bool> HasAccountPasswordGroupRole(Account account, PasswordGroup passwordGroup);
    Task<PasswordGroup> GetAndValidPasswordGroup(Guid passwordGroupId, PasswordGroupType passwordGroupType);
}
