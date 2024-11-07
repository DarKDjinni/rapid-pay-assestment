namespace Distillery.RapidPay.Application.Common.Contracts;

using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<CreditCard> CreditCards { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
