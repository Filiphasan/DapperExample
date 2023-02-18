using System.Text;
using System.Text.RegularExpressions;

namespace Dapper.Api.Middlewares;

public class SqlInjectionMiddleware
{
    private readonly RequestDelegate _next;

    public SqlInjectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method == "GET")
        {
            foreach (var query in context.Request.Query)
            {
                if (HasSqlInjection(query.Value))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid input detected.");
                    return;
                }
            }
        }
        else
        {
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                if (HasSqlInjection(body))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid input detected.");
                    return;
                }

                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            }
        }

        await _next(context);
    }

    private static bool HasSqlInjection(string input)
    {
        var sqlKeywords = new string[] { "select", "insert", "update", "delete", "drop", "create" };
        var sqlRegex = new Regex(string.Join("|", sqlKeywords), RegexOptions.IgnoreCase);
        return sqlRegex.IsMatch(input);
    }
}