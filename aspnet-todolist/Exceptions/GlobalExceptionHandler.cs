using Microsoft.AspNetCore.Diagnostics;

namespace aspnet_todolist.Exceptions
{
    internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing the request.");

            var statusCode = exception switch
            {
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var apiError = new ApiError(
                message: "An error occurred while processing your request.",
                statusCode: statusCode,
                details: exception.Message
            );

            await httpContext.Response.WriteAsJsonAsync(apiError, cancellationToken);

            return true;
        }
    }
}
