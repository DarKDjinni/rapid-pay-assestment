#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using System.Reflection;
using Distillery.RapidPay.Application.Common.Contracts;
using Distillery.RapidPay.Application.Common.Models.Configurations;
using Distillery.RapidPay.CardManagementApi.Services;
using Distillery.RapidPay.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

/// <summary>
/// This class is used to setup all the services required in the CardManagementApi.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// This method will add and configure all the services required in the CardManagementApi.
    /// </summary>
    /// <param name="services">Specifies the service collection used to build the application.</param>
    /// <param name="jwtConfiguration">Is the JWT configuration used by the Auth system.</param>
    /// <param name="isLocal">Boolean that determines if the app is building in Local env.</param>
    /// <param name="useSqlite">Boolean that determines if the app should use Sqlite instead of SqlServer.</param>
    /// <returns>The ServiceCollection including the additional configured services.</returns>
    public static IServiceCollection AddCardManagementApiServices(this IServiceCollection services, JwtConfiguration jwtConfiguration, bool isLocal, bool useSqlite)
    {
        // JWT
        _ = services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtOptions =>
        {
            jwtOptions.Authority = $"https://{jwtConfiguration.Audience}/";
            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidAudience = jwtConfiguration.Audience,
                ValidIssuer = jwtConfiguration.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key))
            };
        });

        if (isLocal && useSqlite)
        {
            _ = services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<SqliteDbContext>()
                .AddDefaultTokenProviders();
        }
        else
        {
            _ = services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<SqlServerDbContext>()
                .AddDefaultTokenProviders();
        }

        // Controllers Configuration
        _ = services.AddControllers();

        // External Implementations
        _ = services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Assembly.GetExecutingAssembly());
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

        _ = services
            .AddExceptionHandler<ExceptionHandler>()
            .AddProblemDetails();

        _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Services
        _ = services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    /// <summary>
    /// This method adds the Configuration values to the WebApp builder pipeline.
    /// </summary>
    /// <param name="builder">A builder for Web Application and Services.</param>
    /// <param name="isLocal">Boolean that determines if the app is building in Local env.</param>
    /// <returns>The WebApp builder including the new configuration.</returns>
    public static WebApplicationBuilder AddBuilderConfiguration(this WebApplicationBuilder builder, bool isLocal)
    {
        // Local Configuration
        if (isLocal)
        {
            _ = builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
        }

        _ = builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)));
        _ = builder.Services.Configure<EncryptionConfiguration>(builder.Configuration.GetSection(nameof(EncryptionConfiguration)));

        return builder;
    }
}
