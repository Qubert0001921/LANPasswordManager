using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Application;

public class ValidationProcessException : Exception
{
    public ValidationProcessException(string message, IEnumerable<ValidationError> errors, Exception? innerException) : base(message, innerException)
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}
