using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Application;

public class ValidationError
{
    public ValidationError(string message, string propertyName, string errorCode)
    {
        Message = message;
        PropertyName = propertyName;
        ErrorCode = errorCode;
    }

    public string Message { get; }
    public string PropertyName { get; }
    public string ErrorCode { get; }

}
