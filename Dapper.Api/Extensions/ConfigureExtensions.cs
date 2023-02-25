using Dapper.Core.Models;
using Dapper.CQRS.Features.Users.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.Api.Extensions;

public static class ConfigureExtensions
{
    public static IMvcBuilder ConfigureCustomApiBehaviors(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response = new GenericResponse<NoDataResponse>()
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ValidationErrors = errors,
                };

                return new BadRequestObjectResult(response);
            };
        });
        return mvcBuilder;
    }

    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddFluentValidationAutoValidation(config =>
        {
            // Disable Data Annotations
            config.DisableDataAnnotationsValidation = true;
        });
        serviceCollection.AddValidatorsFromAssemblyContaining<AddUserCommandValidator>();
        return serviceCollection;
    }
}