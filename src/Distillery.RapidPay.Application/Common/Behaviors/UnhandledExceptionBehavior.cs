namespace Distillery.RapidPay.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogRequestException(typeof(TRequest).Name, request, ex);
            throw;
        }
    }
}
