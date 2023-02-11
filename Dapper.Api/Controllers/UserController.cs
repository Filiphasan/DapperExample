using Dapper.Api.Controllers.BaseControllers;
using Dapper.CQRS.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.Api.Controllers;

public class UserController : CustomControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> AddUser(AddUserCommand request)
    {
        var result = await _mediator.Send(request);
        return CustomResult(result);
    }
}