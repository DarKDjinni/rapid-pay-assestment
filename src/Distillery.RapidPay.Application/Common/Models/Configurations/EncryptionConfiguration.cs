namespace Distillery.RapidPay.Application.Common.Models.Configurations;

public sealed class EncryptionConfiguration
{
    public required string Key { get; init; }
    public required string InitVector { get; init; }
}
