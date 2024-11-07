namespace Distillery.RapidPay.Infrastructure.Services;

using System.Security.Cryptography;
using System.Text;
using Distillery.RapidPay.Application.Common.Models.Configurations;
using Microsoft.Extensions.Options;

internal sealed class EncryptionService(IOptions<EncryptionConfiguration> options) : IEncryptionService
{
    private readonly EncryptionConfiguration encryptionConfiguration = options.Value;

    public Task<string> EncryptAsync(string input)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(this.encryptionConfiguration.Key);
        aes.IV = Encoding.UTF8.GetBytes(this.encryptionConfiguration.InitVector);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(input);
        }

        return Task.FromResult(Convert.ToBase64String(ms.ToArray()));
    }

    public Task<string> DecryptAsync(string input)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(this.encryptionConfiguration.Key);
        aes.IV = Encoding.UTF8.GetBytes(this.encryptionConfiguration.InitVector);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(input));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return Task.FromResult(sr.ReadToEnd());
    }
}
