using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Services.Accounts;

public interface IAccountHelperService
{
    Task<Account> CheckIfAccountExists(Guid id);
}
