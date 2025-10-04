using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Core.Responses;
using System.Net;
using System.Text.Json;

namespace Linkdev.TeamTrack.API.Middlewares
{

    public class UnifiedResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonOptions;

        public UnifiedResponseMiddleware(RequestDelegate next)
        {
            _next = next;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null &&
                (path.StartsWith("/swagger") ||
                 path.StartsWith("/favicon") ||
                 path.Contains(".") || 
                 !path.StartsWith("/api")))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using var newBodyStream = new MemoryStream();
            context.Response.Body = newBodyStream;

            try
            {
                await _next(context);

                newBodyStream.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(newBodyStream).ReadToEndAsync();
                newBodyStream.Seek(0, SeekOrigin.Begin);

                object? originalData = null;

                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    try
                    {
                        originalData = JsonSerializer.Deserialize<object>(bodyText, _jsonOptions);
                    }
                    catch
                    {
                        originalData = bodyText;
                    }
                }

                var responseWrapper = new Response<object>
                {
                    StatusCode = context.Response.StatusCode,
                    Message = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300
                        ? "Success"
                        : "Error",
                    Data = originalData
                };

                context.Response.ContentType = "application/json";
                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(JsonSerializer.Serialize(responseWrapper, _jsonOptions));
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<object>
                {
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { ex.Message }
                };

                errorResponse.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.Body = originalBodyStream;
                context.Response.StatusCode = errorResponse.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, _jsonOptions));
            }
        }
    }

}
