using Dapper.Core.Models;
using MediatR;

namespace Dapper.CQRS.Models.Auth;

public class LoginCommand : IRequest<GenericResponse<LoginResponse>>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; }
    public int ExpireInSeconds { get; set; }
    public DateTime CreateDateTime { get; set; }
}