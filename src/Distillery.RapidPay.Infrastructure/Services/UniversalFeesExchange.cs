namespace Distillery.RapidPay.Infrastructure.Services;

internal sealed class UniversalFeesExchange(TimeProvider timeProvider) : IUniversalFeesExchange
{
    private decimal fee = GetBaseFee();
    private DateTimeOffset latestUpdate = DateTimeOffset.MinValue;

    public Task<decimal> GetFeeAsync()
    {
        if ((timeProvider.GetUtcNow() - this.latestUpdate).TotalHours > 1)
        {
            this.latestUpdate = timeProvider.GetUtcNow();
            this.fee = Math.Round(this.fee * GetBaseFee(), 2);
        }

        return Task.FromResult(this.fee);
    }

    private static decimal GetBaseFee() => (decimal)Math.Round(Random.Shared.NextDouble() * 2, 2);
}
