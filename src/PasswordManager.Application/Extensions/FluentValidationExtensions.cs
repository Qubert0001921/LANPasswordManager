using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Extensions;

public static class FluentValidationExtensions
{
    public static async Task ValidateAndThrowValidationProcessExceptionAsync<T>(this IValidator<T> validator, T model)
    {
        var result = await validator.ValidateAsync(model);

        if(!result.IsValid)
        {
            var exception = new ValidationException(result.Errors);
            
            var errors = result.Errors.Select(
                x => new ValidationError(
                    x.ErrorMessage,
                    x.PropertyName,
                    x.ErrorCode
                ));

            throw new ValidationProcessException(exception.Message, errors, exception);
        }
    }
}
