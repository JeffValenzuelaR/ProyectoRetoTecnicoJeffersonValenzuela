using EventService.Application.Common.Exceptions;
using EventService.Domain.Exceptions;
using FluentValidation;

namespace EventService.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(exception, "Excepcion no controlada. TraceId: {TraceId}", traceId);

            var (statusCode, message, errors) = Map(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            if (errors is not null)
            {
                await context.Response.WriteAsJsonAsync(new { traceId, message, errors });
            }
            else
            {
                await context.Response.WriteAsJsonAsync(new { traceId, message });
            }
        }
    }

    private static (int StatusCode, string Message, IReadOnlyDictionary<string, string[]>? Errors) Map(Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return (StatusCodes.Status400BadRequest, "Uno o mas campos no son validos.", errors);

            case NotFoundException notFoundException:
                return (StatusCodes.Status404NotFound, notFoundException.Message, null);

            case DomainException domainException:
                return (StatusCodes.Status400BadRequest, domainException.Message, null);

            default:

                return (
                    StatusCodes.Status500InternalServerError,
                    "Ocurrio un error inesperado. Si el problema persiste, contacte a soporte indicando el traceId.",
                    null);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
