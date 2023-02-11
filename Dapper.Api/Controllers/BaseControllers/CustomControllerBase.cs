using Dapper.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.Api.Controllers.BaseControllers;

[Route("api/[controller]")]
[ApiController]
public class CustomControllerBase : ControllerBase
{
    [NonAction]
    protected IActionResult CustomResult<T>(GenericResponse<T> response) where T: class, new()
    {
        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}