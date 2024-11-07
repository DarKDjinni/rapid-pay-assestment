namespace Distillery.RapidPay.Application.Common.Contracts;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string input);
    Task<string> DecryptAsync(string input);
}
