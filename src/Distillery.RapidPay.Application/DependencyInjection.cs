#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Distillery.RapidPay.Application.Common.Behaviors;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());
        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        _ = services.AddMediatR(config =>
        {
            _ = config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            _ = config
                .AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>))
                .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
