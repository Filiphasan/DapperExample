using Dapper.Core.Models;
using MediatR;

namespace Dapper.CQRS.Models.Users;

public class AddUserCommand : IRequest<GenericResponse<AddUserResponse>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class AddUserResponse
{
    public string Message { get; set; }
}