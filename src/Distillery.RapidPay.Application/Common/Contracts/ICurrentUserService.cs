namespace Distillery.RapidPay.Application.Common.Contracts;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
}
