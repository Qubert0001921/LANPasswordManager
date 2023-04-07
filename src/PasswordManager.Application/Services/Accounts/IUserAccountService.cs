using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Services.Accounts;

public interface IUserAccountService
{
    Task CreateUserAccount(AccountDto dto, AccountDto creator);
}
