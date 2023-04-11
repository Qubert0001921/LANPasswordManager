using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using PasswordManager.Application.Dtos;

namespace PasswordManager.Application.Validators;

public class RoleDtoValidator : AbstractValidator<RoleDto>
{
    public RoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("'{PropertyName}' musn't be empty")
            .NotNull().WithMessage("'{PropertyName}' musn't be empty")
            .Must(SharedValidation.BeAValidName).WithMessage("{PropertyName} contains invalid characters");
    }
}
