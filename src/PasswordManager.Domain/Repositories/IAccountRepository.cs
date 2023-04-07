using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Entities;

namespace PasswordManager.Domain.Repositories;

public interface IAccountRepository : IBaseRepository<Account, Guid>
{
    Task<IEnumerable<Account>> GetAllAdminAccountsAsync();
    Task<Account> GetAdminAccountByIdAsync(Guid id);
    Task<Account> GetAccountByLoginAsync(string login);
    Task<Account> GetUserAccountByIdAsync(Guid id);
}
