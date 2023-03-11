using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Models;

public class PasswordGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PasswordGroupIconId { get; set; }
    public string RoleId { get; set; }
}
