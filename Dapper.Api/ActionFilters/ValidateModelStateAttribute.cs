using Dapper.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dapper.Api.ActionFilters;

public class ValidateModelStateAttribute : ActionFilterAttribute 
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new GenericResponse<NoDataResponse>()
            {
                StatusCode = 400,
                ValidationErrors = errors,
            };

            var result = new BadRequestObjectResult(response);
            context.Result = result;
        }
    }
}