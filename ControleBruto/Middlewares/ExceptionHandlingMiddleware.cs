using ControleBruto.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ControleBruto.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var (status, title) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                    BusinessRuleException => (StatusCodes.Status400BadRequest, "Business rule violation"),
                    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
                };

                var problem = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = ex.Message,
                    Type = $"https://httpstatuses.com/{status}",
                    Instance = context.Request.Path
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = status;

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
