namespace Distillery.RapidPay.Application.Common.Loggers;

using Distillery.RapidPay.Domain.Common;

public static partial class CommonLoggerMessages
{
    [LoggerMessage(EventId = LogGroupEventIds.CommonEvent + 1, Level = LogLevel.Information, Message = "Handling {RequestName}: {@Request}")]
    public static partial void LogHandlingRequest(this ILogger logger, string requestName, dynamic request);

    [LoggerMessage(EventId = LogGroupEventIds.CommonEvent + 2, Level = LogLevel.Information, Message = "Handled {RequestName}")]
    public static partial void LogRequestHandled(this ILogger logger, string requestName);

    [LoggerMessage(EventId = LogGroupEventIds.ExceptionEvent + 1, Level = LogLevel.Error, Message = "Unhandled Exception {ExceptionId} in {Context}: {Message}")]
    public static partial void LogGenericException(this ILogger logger, Guid exceptionId, string context, string message, Exception ex);

    [LoggerMessage(EventId = LogGroupEventIds.ExceptionEvent + 2, Level = LogLevel.Error, Message = "Unhandled Exception for Request {RequestName} {@Request}")]
    public static partial void LogRequestException(this ILogger logger, string requestName, dynamic request, Exception ex);
}
