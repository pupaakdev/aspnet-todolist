using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.Exceptions
{
    internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {ExceptionType}", exception.GetType().Name);

            var (statusCode, title, type) = exception switch
            {
                ValidationException validationEx => 
                    (StatusCodes.Status400BadRequest, "Validation failed.", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

                DbUpdateException dbUpdateEx => 
                    (StatusCodes.Status409Conflict, "A database conflict occurred. The operation could not be completed.", "https://tools.ietf.org/html/rfc9110#section-15.5.10"),

                ArgumentException or ArgumentNullException => 
                    (StatusCodes.Status400BadRequest, "Invalid argument provided.", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

                KeyNotFoundException => 
                    (StatusCodes.Status404NotFound, "The requested resource was not found.", "https://tools.ietf.org/html/rfc9110#section-15.5.5"),

                UnauthorizedAccessException => 
                    (StatusCodes.Status403Forbidden, "Access to this resource is forbidden.", "https://tools.ietf.org/html/rfc9110#section-15.5.4"),

                InvalidOperationException => 
                    (StatusCodes.Status400BadRequest, "The requested operation is invalid.", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),

                _ => 
                    (StatusCodes.Status500InternalServerError, "An error occurred while processing your request.", "https://tools.ietf.org/html/rfc9110#section-15.6.1")
            };

            httpContext.Response.StatusCode = statusCode;

            var errors = new Dictionary<string, string[]>
            {
                [exception.GetType().Name] = [exception.Message]
            };

            var problemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = statusCode,
                Extensions =
                {
                    ["errors"] = errors
                }
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
