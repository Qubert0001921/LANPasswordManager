using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Domain.Repositories;

public interface IPasswordGroupRepository : IBaseRepository<PasswordGroup, Guid>
{
    Task AddPasswordToPasswordGroupAsync(PasswordGroup passwordGroup, Password password);
    Task RemovePasswordFromPasswordGroupAsync(PasswordGroup passwordGroup, Password password);
    Task<IEnumerable<PasswordGroup>> GetChildrenOfPasswordGroupAsync(PasswordGroup passwordGroup);

    Task<IEnumerable<PasswordGroup>> GetAllChildPasswordGroupsAsync();
    Task<IEnumerable<PasswordGroup>> GetAllMainPasswordGroupsAsync();
}
