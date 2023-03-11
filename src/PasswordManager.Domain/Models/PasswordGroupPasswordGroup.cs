using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Models;

public class PasswordGroupPasswordGroup
{
    public string Id { get; set; }
    public string ParentPasswordGroupId { get; set; }
    public string ChildPasswordGroupId { get; set; }
}
