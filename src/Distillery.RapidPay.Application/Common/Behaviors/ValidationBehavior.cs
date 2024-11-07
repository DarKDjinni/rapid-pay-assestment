namespace Distillery.RapidPay.Application.Common.Behaviors;

using Ardalis.Result.FluentValidation;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var validatorErrors = validationResults
                .Select(vr => vr.AsErrors())
                .SelectMany(ve => ve)
                .ToList();

            if (validatorErrors.Count != 0)
            {
                var type = typeof(TResponse).GetGenericArguments().FirstOrDefault();

                if (type != null)
                {
                    var resultType = typeof(Result<>).MakeGenericType(type);
                    var resultCreate = resultType.GetMethod("Invalid", [typeof(IEnumerable<ValidationError>)]) ?? throw new NotImplementedException("ValidationBehavior dependency Result.Invalid not implemented.");

                    return (TResponse)resultCreate.Invoke(null, [validatorErrors])!;
                }

                return (TResponse)(object)Result.Invalid(validatorErrors);
            }
        }

        return await next();
    }
}
