using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Entities;

public class Password
{
    public Password(Guid id, string passwordCipher)
    {
        Id = id;
        PasswordCipher = passwordCipher;
    }

    public Guid Id { get; private init; }
    public string PasswordCipher { get; private set; }
}
