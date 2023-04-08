using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IPasswordGroupHelperService
{
    Task<Password> CheckIfPasswordExists(Guid id);
    Task<PasswordGroup> CheckIfPasswordGroupExists(Guid id);

    Task<IEnumerable<Guid>> GetAllChildrenIdsOfPasswordGroup(PasswordGroup passwordGroup);
    Task<IEnumerable<PasswordGroup>> GetAllChildrenOfPasswordGroup(PasswordGroup passwordGroup);

    Task<IEnumerable<Guid>> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup);
    IEnumerable<Guid> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup, IEnumerable<PasswordGroup> childrenPasswordGroups);

    Task<bool> HasAccountPasswordGroupRole(Account account, PasswordGroup passwordGroup);

    Task CheckAccountAndPasswordGroupRoles(Account account, PasswordGroup passwordGroup);
}
