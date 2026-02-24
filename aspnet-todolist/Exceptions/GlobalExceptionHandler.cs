using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.Exceptions
{
    internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {ExceptionType}", exception.GetType().Name);

            var (statusCode, message) = exception switch
            {
                ValidationException validationEx => 
                    (StatusCodes.Status400BadRequest, "Validation failed."),

                DbUpdateException dbUpdateEx => 
                    (StatusCodes.Status409Conflict, "A database conflict occurred. The operation could not be completed."),

                ArgumentException or ArgumentNullException => 
                    (StatusCodes.Status400BadRequest, "Invalid argument provided."),

                KeyNotFoundException => 
                    (StatusCodes.Status404NotFound, "The requested resource was not found."),

                UnauthorizedAccessException => 
                    (StatusCodes.Status403Forbidden, "Access to this resource is forbidden."),

                InvalidOperationException => 
                    (StatusCodes.Status400BadRequest, "The requested operation is invalid."),

                _ => 
                    (StatusCodes.Status500InternalServerError, "An error occurred while processing your request.")
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var apiError = new ApiError(
                message: message,
                statusCode: statusCode,
                details: exception.Message
            );

            await httpContext.Response.WriteAsJsonAsync(apiError, cancellationToken);

            return true;
        }
    }
}
