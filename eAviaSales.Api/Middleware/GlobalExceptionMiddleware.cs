using eAviaSales.Api.Contracts.Common;
using eAviaSales.Api.Contracts.Errors;

namespace eAviaSales.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception for path {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var message = _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected server error occurred.";

            var response = ApiResponse<object>.Fail(
                new ApiError
                {
                    Code = ApiErrorCodes.InternalServerError,
                    Message = message
                },
                context.TraceIdentifier);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
