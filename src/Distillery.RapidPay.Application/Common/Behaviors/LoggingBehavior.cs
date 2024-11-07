namespace Distillery.RapidPay.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogHandlingRequest(typeof(TRequest).Name, request);
        var response = await next();
        logger.LogRequestHandled(typeof(TRequest).Name);

        return response;
    }
}
