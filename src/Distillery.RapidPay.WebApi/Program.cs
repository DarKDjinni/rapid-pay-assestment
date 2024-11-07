using Distillery.RapidPay.Application.Common.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configuration
bool isLocal = Environment.GetEnvironmentVariable("IS_LOCAL") == "true";
builder.AddBuilderConfiguration(isLocal);
bool useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");

// JWT Configuration
var jwtConfigurationSection = builder.Configuration.GetSection(nameof(JwtConfiguration));
var jwtConfiguration = jwtConfigurationSection.Get<JwtConfiguration>();
ArgumentNullException.ThrowIfNull(jwtConfiguration);

// Layer Services
builder.Services.AddApplicationServices();
builder.Services.AddCardManagementApiServices(jwtConfiguration, isLocal, useSqlite);
builder.Services.AddInfrastructureServices(builder.Configuration, isLocal, useSqlite);

// Application setup
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

_ = app.UseHttpsRedirection();
_ = app.UseAuthentication();
_ = app.UseAuthorization();

_ = app.UseExceptionHandler();

_ = app.MapControllers();

await app.RunAsync();
