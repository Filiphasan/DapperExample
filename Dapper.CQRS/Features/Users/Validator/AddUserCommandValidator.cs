using Dapper.CQRS.Models.Users;
using FluentValidation;

namespace Dapper.CQRS.Features.Users.Validator;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First Name is required.");
        RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last Name is required.");
        RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required.");
    }
}