using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Services.PasswordGroups;

public interface IPasswordGroupHelperService
{
    Task<Account> CheckIfAccountExists(Guid id);
    void ThrowIfNotAdmin(Account account, string message);
    Task<Password> CheckIfPasswordExists(Guid id);

    Task<IEnumerable<Guid>> GetAllChildrenIdsOfPasswordGroup(PasswordGroup passwordGroup);
    Task<IEnumerable<PasswordGroup>> GetAllChildrenOfPasswordGroup(PasswordGroup passwordGroup);

    Task<IEnumerable<Guid>> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup);
    IEnumerable<Guid> GetAllPasswordIdsOfPasswordGroup(PasswordGroup passwordGroup, IEnumerable<PasswordGroup> childrenPasswordGroups);
}
