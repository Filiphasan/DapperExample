using System.Text;
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

        // Log incoming request
        LogRequest(context.Request, correlationId);

        // Store the original body stream
        var originalBodyStream = context.Response.Body;

        // Create a new memory stream to replace the original response stream
        using (var responseBody = new MemoryStream())
        {
            // Replace the response stream
            context.Response.Body = responseBody;

            // Call the next middleware
            await _next(context);

            // Log the response after it has been processed
            await LogResponse(context.Response, originalBodyStream, correlationId);
        }
    }

    private void LogRequest(HttpRequest request, string correlationId)
    {
        var stringBuilder = new StringBuilder();

        // Log the request method, URI, and headers
        stringBuilder.AppendLine("CorrelationId: " + correlationId);
        stringBuilder.AppendLine("Request method: " + request.Method);
        stringBuilder.AppendLine("Request URI: " + request.GetDisplayUrl());
        stringBuilder.AppendLine("Request headers: " + request.Headers.ToString());

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
        var requestBody = reader.ReadToEnd();
        request.Body.Position = 0;
        stringBuilder.AppendLine("Request body: " + MaskSensitiveData(requestBody));
        
        _logger.LogInformation(stringBuilder.ToString());
    }

    private async Task LogResponse(HttpResponse response, Stream originalBodyStream, string correlationId)
    {
        var stringBuilder = new StringBuilder();

        // Store the response body in a memory stream
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();

        // Reset the response stream to the original stream
        response.Body = originalBodyStream;

        // Log the response status code and headers
        stringBuilder.AppendLine("CorrelationId: " + correlationId);
        stringBuilder.AppendLine("Response status code: " + response.StatusCode);
        stringBuilder.AppendLine("Response headers: " + response.Headers.ToString());

        // Log the response body, masking sensitive data
        stringBuilder.AppendLine("Response body: " + MaskSensitiveData(responseBody));

        _logger.LogInformation(stringBuilder.ToString());
    }

    private string MaskSensitiveData(string payload)
    {
        payload = payload.Replace("\"password\":\"[^\"]+\"", "\"password\":\"***\"");

        return payload;
    }
}