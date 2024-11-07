namespace Distillery.RapidPay.CardManagementApi.Infrastructure;

using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

internal static class ResultExtension
{
    public static IActionResult ToApiResult<T>(this Result<T> result) => ((IResult)result).ToApiResult();

    public static IActionResult ToApiResult(this Result result) => ((IResult)result).ToApiResult();

    private static IActionResult ToApiResult(this IResult result) => result.Status switch
    {
        ResultStatus.Ok => (result is Result) ? new OkResult() : new OkObjectResult(result.GetValue()),
        ResultStatus.Created => new CreatedResult(string.Empty, result.GetValue()),
        ResultStatus.Error => UnprocessableEntity(result),
        ResultStatus.Forbidden => Forbidden(result),
        ResultStatus.Unauthorized => Unauthorized(result),
        ResultStatus.Invalid => InvalidEntity(result),
        ResultStatus.NotFound => NotFoundEntity(result),
        ResultStatus.NoContent => new NoContentResult(),
        ResultStatus.Conflict => ConflictEntity(result),
        ResultStatus.CriticalError => CriticalEntity(result),
        ResultStatus.Unavailable => UnavailableEntity(result),
        _ => throw new NotSupportedException($"Result {result.Status} conversion is not supported."),
    };

    private static UnprocessableEntityObjectResult UnprocessableEntity(IResult result)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = GetErrorMessage(result.Errors),
            Title = "Something went wrong."
        };

        return new UnprocessableEntityObjectResult(problemDetails);
    }

    private static IActionResult NotFoundEntity(IResult result)
    {
        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "Resource not found."
            };

            return new NotFoundObjectResult(problemDetails);
        }

        return new NotFoundResult();
    }

    private static IActionResult ConflictEntity(IResult result)
    {
        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "There was a conflict."
            };

            return new ConflictObjectResult(problemDetails);
        }

        return new ConflictResult();
    }

    private static IActionResult CriticalEntity(IResult result)
    {
        int httpCodeInternalServerError = (int)HttpStatusCode.InternalServerError;

        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "Something went wrong.",
                Status = httpCodeInternalServerError
            };

            return new ObjectResult(problemDetails);
        }

        return new StatusCodeResult(httpCodeInternalServerError);
    }

    private static IActionResult UnavailableEntity(IResult result)
    {
        int httpCodeServiceUnavailable = (int)HttpStatusCode.ServiceUnavailable;

        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "Service unavailable.",
                Status = httpCodeServiceUnavailable
            };

            return new ObjectResult(problemDetails);
        }

        return new StatusCodeResult(httpCodeServiceUnavailable);
    }

    private static IActionResult Forbidden(IResult result)
    {
        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "Forbidden.",
                Status = (int)HttpStatusCode.Forbidden
            };

            return new ObjectResult(problemDetails);
        }

        return new ForbidResult();
    }

    private static IActionResult Unauthorized(IResult result)
    {
        if (result.Errors.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(result.Errors),
                Title = "Unauthorized."
            };

            return new UnauthorizedObjectResult(problemDetails);
        }

        return new UnauthorizedResult();
    }

    private static BadRequestObjectResult InvalidEntity(IResult result)
    {
        var errors = result.ValidationErrors
            .GroupBy(ve => ve.Identifier)
            .ToDictionary(g => g.Key, g => g.Select(ve => ve.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new ValidationProblemDetails(errors));
    }

    private static string GetErrorMessage(IEnumerable<string> errors)
    {
        var stringBuilder = new StringBuilder("Next error(s) occurred:");
        foreach (string error in errors)
        {
            _ = stringBuilder.Append('|').Append(error).AppendLine();
        }
        return stringBuilder.ToString();
    }
}
