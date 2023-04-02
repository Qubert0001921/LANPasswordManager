using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Application.Dtos;

public class PasswordDto
{
    public Guid Id { get; set; }
    public string PasswordCipher { get; set; }
}
