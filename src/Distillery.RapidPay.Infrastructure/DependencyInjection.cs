#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Distillery.RapidPay.Application.Common.Contracts;
using Distillery.RapidPay.Infrastructure.Persistance;
using Distillery.RapidPay.Infrastructure.Persistance.Interceptors;
using Distillery.RapidPay.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isLocal, bool useSqlite)
    {
        // Database
        _ = services.AddScoped<ISaveChangesInterceptor, RecordOwnerInterceptor>();

        if (isLocal && useSqlite)
        {
            Console.WriteLine("SQLITE");

            string? rapidPaySqliteConnectionString = configuration.GetConnectionString("RapidPaySqlite");
            ArgumentException.ThrowIfNullOrWhiteSpace(rapidPaySqliteConnectionString);

            _ = services.AddDbContext<SqliteDbContext>((sp, option) =>
            {
                _ = option.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                _ = option.UseSqlite(rapidPaySqliteConnectionString);
            });
            _ = services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SqliteDbContext>());
        }
        else
        {
            Console.WriteLine("SQLSERVER");

            string? rapidPaySqlServerConnectionString = configuration.GetConnectionString("RapidPaySqlServer");
            ArgumentException.ThrowIfNullOrWhiteSpace(rapidPaySqlServerConnectionString);

            Console.WriteLine(rapidPaySqlServerConnectionString);

            _ = services.AddDbContext<SqlServerDbContext>((sp, option) =>
            {
                _ = option.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                _ = option.UseSqlServer(rapidPaySqlServerConnectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null));
            });
            _ = services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SqlServerDbContext>());
        }

        // Services
        _ = services.AddSingleton(TimeProvider.System);
        _ = services.AddSingleton<IUniversalFeesExchange, UniversalFeesExchange>();
        _ = services.AddSingleton<IJsonWebTokenService, JsonWebTokenService>();
        _ = services.AddTransient<IEncryptionService, EncryptionService>();

        return services;
    }
}
