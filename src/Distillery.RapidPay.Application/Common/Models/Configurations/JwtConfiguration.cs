namespace Distillery.RapidPay.Application.Common.Models.Configurations;

public sealed class JwtConfiguration
{
    public required string Audience { get; init; }
    public required string Issuer { get; init; }
    public required string Key { get; init; }
}
