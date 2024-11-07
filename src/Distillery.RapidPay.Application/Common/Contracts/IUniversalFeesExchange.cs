namespace Distillery.RapidPay.Application.Common.Contracts;

public interface IUniversalFeesExchange
{
    Task<decimal> GetFeeAsync();
}
