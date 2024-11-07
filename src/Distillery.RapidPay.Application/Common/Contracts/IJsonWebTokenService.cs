namespace Distillery.RapidPay.Application.Common.Contracts;

using Microsoft.AspNetCore.Identity;

public interface IJsonWebTokenService
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}
