using System.Text.Json;
using TheBand.AuthApplication.Exceptions;

namespace TheBand.AuthApi.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = 400;

            var response = Result<object>.Failure(ex.Message);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical Error");

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = 500;

            var response = new Result<object>
            {
                Success = false,
                Message = "Internal Server Error.",
                Errors = ex.ToString()
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
