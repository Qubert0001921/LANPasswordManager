using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using PasswordManager.Application.Dtos;
using PasswordManager.Application.Extensions;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application.Validators;

public class PasswordGroupDtoValidator : AbstractValidator<PasswordGroupDto>
{
    public PasswordGroupDtoValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("'{PropertyName}' musn't be empty")
            .NotNull().WithMessage("'{PropertyName}' musn't be empty")
            .Length(2, 50).WithMessage("Length of '{PropertyName}' is invalid")
            .Must(SharedValidation.BeAValidName).WithMessage("'{PropertyName}' contains invalid characters");

        RuleFor(x => x.AccessRoles)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("'{PropertyName}' musn't be empty")
            .Must(SharedValidation.CountBeGreaterThanZero).WithMessage("'{PropertyName}' must has at least one role")
            .When(x => x.PasswordGroupType == PasswordGroupType.Main);

        RuleFor(x => x.Passwords)
            .NotNull().WithMessage("'{PropertyName}' musn't be empty");

        RuleForEach(x => x.Passwords)
            .SetValidator(new PasswordDtoValidator());

        RuleFor(x => x.ParentPasswordGroupId)
            .Must(x => x != Guid.Empty).WithMessage("'{PropertyName}' musn't be empty guid")
            .When(x => x.PasswordGroupType == PasswordGroupType.Child);
    }
}
