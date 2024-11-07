namespace Distillery.RapidPay.CardManagementApi.Services;

using System.Security.Claims;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public string UserId => new(this.httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public string UserName => new(this.httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)!);
}
