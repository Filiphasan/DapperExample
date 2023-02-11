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
}