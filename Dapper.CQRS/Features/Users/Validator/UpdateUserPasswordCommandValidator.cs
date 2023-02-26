using Dapper.Core.Constants;
using Dapper.CQRS.Models.Users;
using FluentValidation;

namespace Dapper.CQRS.Features.Users.Validator;

public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(x => x.Password).NotNull().WithMessage("Password is required").NotEmpty()
            .WithMessage("Password is Required");
        RuleFor(x => x.OldPassword).NotNull().WithMessage("Old Password is required").NotEmpty()
            .WithMessage("Old Password is required");
        RuleFor(x => x.Password).Matches(RegexConstants.PasswordRegex).WithMessage(RegexConstants.PasswordRegexMessage);
        RuleFor(x => x.OldPassword).Matches(RegexConstants.PasswordRegex).WithMessage(RegexConstants.PasswordRegexMessage);
    }
}