using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Domain.Repositories;

public interface IPasswordGroupRepository : IBaseRepository<PasswordGroup, Guid>
{
    Task AddPasswordToPasswordGroup(PasswordGroup passwordGroup, Password password);
    Task RemovePasswordFromPasswordGroup(PasswordGroup passwordGroup, Password password);
}
