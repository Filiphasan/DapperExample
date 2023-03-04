using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;

namespace Dapper.Api.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private const string CorrelationIdKey = "X-CorrelationId";

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdKey].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdKey, correlationId);
            return Task.CompletedTask;
        });

        await LogRequest(context.Request, correlationId);

        var originalBodyStream = context.Response.Body;

        await using var responseBody = new MemoryStream();

        context.Response.Body = responseBody;

        await _next(context);

        await LogResponse(context.Response, originalBodyStream, correlationId);
    }

    private async Task LogRequest(HttpRequest request, string correlationId)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("CorrelationId: " + correlationId);
        stringBuilder.AppendLine("Request method: " + request.Method);
        stringBuilder.AppendLine("Request URI: " + request.GetDisplayUrl());
        stringBuilder.AppendLine("Request headers: " + request.Headers);

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
        var requestBody = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        stringBuilder.AppendLine("Request body: " + MaskSensitiveData(requestBody));

        _logger.LogInformation(stringBuilder.ToString());
    }

    private async Task LogResponse(HttpResponse response, Stream originalBodyStream, string correlationId)
    {
        var stringBuilder = new StringBuilder();

        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body, Encoding.UTF8).ReadToEndAsync();

        response.Body = originalBodyStream;

        stringBuilder.AppendLine("CorrelationId: " + correlationId);
        stringBuilder.AppendLine("Response status code: " + response.StatusCode);
        stringBuilder.AppendLine("Response headers: " + response.Headers);
        stringBuilder.AppendLine("Response body: " + MaskSensitiveData(responseBody));

        _logger.LogInformation(stringBuilder.ToString());
    }

    private string MaskSensitiveData(string payload)
    {
        payload = Regex.Replace(payload, "\"password\"\\s*:\\s*\"(.*?)\"", "\"password\":\"******\"");

        return payload;
    }
}