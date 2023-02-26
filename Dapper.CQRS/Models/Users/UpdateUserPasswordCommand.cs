using Dapper.Core.Models;
using MediatR;

namespace Dapper.CQRS.Models.Users;

public class UpdateUserPasswordCommand : IRequest<GenericResponse<UpdateUserPasswordResponse>>
{
    public string OldPassword { get; set; }
    public string Password { get; set; }
}

public class UpdateUserPasswordResponse
{
    public string Message { get; set; }
}