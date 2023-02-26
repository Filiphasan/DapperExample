using Dapper.Api.Controllers.BaseControllers;
using Dapper.Core.Models;
using Dapper.CQRS.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.Api.Controllers;

public class AuthController : CustomControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ProducesResponseType(typeof(GenericResponse<LoginResponse>) ,200)]
    [ProducesResponseType(typeof(GenericResponse<NoDataResponse>) ,400)]
    [ProducesResponseType(typeof(GenericResponse<NoDataResponse>) ,500)]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand request)
    {
        return CustomResult(await _mediator.Send(request));
    }
}