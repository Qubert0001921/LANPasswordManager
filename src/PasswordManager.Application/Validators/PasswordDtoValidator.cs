using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Validators;

public class PasswordDtoValidator : AbstractValidator<PasswordDto>
{
    public PasswordDtoValidator()
    {
        RuleFor(x => x.PasswordCipher)
            .NotEmpty().WithMessage("'{PropertyName}' musn't be empty")
            .NotNull().WithMessage("'{PropertyName}' musn't be empty")
            .MinimumLength(0).WithMessage("'{PropertyName}' lenght must be greater than 0");
    }
}
