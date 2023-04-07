using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.Accounts;

public interface IAdminAccountService
{
    Task CreateAdminAccount(AccountDto dto, AccountDto creator);
}
