using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace Linkdev.TeamTrack.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled Exception Occurred");

            var response = new ErrorResponse()
            {
                Message = exception.Message
            };

            response.StatusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(response , cancellationToken);
            return true;
        }
    }
}
