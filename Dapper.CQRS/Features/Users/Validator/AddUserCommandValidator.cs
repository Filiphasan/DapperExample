using Dapper.CQRS.Models.Users;
using FluentValidation;

namespace Dapper.CQRS.Features.Users.Validator;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotNull().WithMessage("First Name is required.").NotEmpty().WithMessage("First Name is required.");
        RuleFor(x => x.LastName).NotNull().WithMessage("Last Name is required.").NotEmpty().WithMessage("Last Name is required.");
        RuleFor(x => x.UserName).NotNull().WithMessage("Username is required.").NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotNull().WithMessage("Password is required.").NotEmpty().WithMessage("Password is required.");
    }
}