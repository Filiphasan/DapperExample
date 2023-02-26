using Dapper.Core.Models;
using Microsoft.Extensions.Logging;

namespace Dapper.CQRS;

public class BaseHandler<THandler>
{
    // ReSharper disable once InconsistentNaming
    protected readonly ILogger<THandler> _logger;

    public BaseHandler(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<THandler>();
    }

    protected GenericResponse<TResponse> HandleExceptionResponse<TResponse>(Exception exception) where TResponse: class, new()
    {
        var result = GenericResponse<TResponse>.Error(500, "Something went wrong!");
        _logger.LogError(exception, "Error RequestId: {RequestId}", result.Uuid);
        return result;
    }
    protected GenericResponse<TResponse> HandleExceptionResponse<TResponse>(Exception exception, string message) where TResponse: class, new()
    {
        var result = GenericResponse<TResponse>.Error(500, message);
        _logger.LogError(exception, "Error RequestId: {RequestId}", result.Uuid);
        return result;
    }
}