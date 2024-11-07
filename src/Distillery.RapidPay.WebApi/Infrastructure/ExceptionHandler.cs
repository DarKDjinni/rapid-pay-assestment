namespace Distillery.RapidPay.CardManagementApi.Infrastructure;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

/// <summary>
/// Used to encapsulated the exception handling routines.
/// </summary>
/// <param name="logger">An instance of ILogger.</param>
public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Handles all unexpected exceptions, logs the information and returns an user-friendly message.
    /// </summary>
    /// <param name="httpContext">Encapsulates the HTTP-specific information.</param>
    /// <param name="exception">Represents errors during the app exectuion.</param>
    /// <param name="cancellationToken">Propagates the notifications to cancel an operation.</param>
    /// <returns>true if the exception was handled.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionId = Guid.NewGuid();
        logger.LogGenericException(exceptionId, GetMethodNameFromException(exception), exception.Message, exception);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await Results.Problem(
            title: "Internal Server Error",
            detail: $"Please contact an administrator and provide this Id {exceptionId}",
            statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(httpContext);

        return true;
    }

    private static string GetMethodNameFromException(Exception ex)
    {
        var stackTrace = new StackTrace(ex, true);
        var stackFrame = stackTrace.GetFrame(0);

        return stackFrame?.GetMethod()?.Name ?? "Unknown method";
    }
}
