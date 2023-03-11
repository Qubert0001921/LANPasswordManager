using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Models;

public class Password
{
    public string Id { get; set; }
    public string PasswordCipher { get; set; }
    public string PasswordGrouId { get; set; }
}
