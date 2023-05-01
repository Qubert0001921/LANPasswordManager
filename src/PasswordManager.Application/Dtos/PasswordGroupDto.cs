using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Dtos;

public class PasswordGroupDto
{
    public Guid Id { get; set; }
    public string Name { get;  set; }
    public List<RoleDto> AccessRoles { get;  set; }
    public List<PasswordDto> Passwords { get; set; }
    public Guid ParentPasswordGroupId { get;  set; }
}
