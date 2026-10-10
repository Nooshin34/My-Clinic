using Clinic.Application.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Clinic.Api.ExceptionHandling;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, errors) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, notFound.Message, (IReadOnlyList<string>?)null),
            AppValidationException validation => (StatusCodes.Status400BadRequest, "اطلاعات واردشده معتبر نیست.", validation.Errors),
            _ => (StatusCodes.Status500InternalServerError, "خطای غیرمنتظره رخ داد.", null)
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            title,
            status,
            errors
        }, cancellationToken);

        return true;
    }
}
