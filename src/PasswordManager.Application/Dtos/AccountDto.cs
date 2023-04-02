using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Application.Dtos;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<RoleDto> Roles { get; set; }
    public bool IsAdmin { get; set; }
}
