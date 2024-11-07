namespace Distillery.RapidPay.Infrastructure.Persistance.Common;

using System.Reflection;
using Distillery.RapidPay.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public abstract class BaseDbContext(DbContextOptions options) : IdentityDbContext(options), IApplicationDbContext
{
    public DbSet<CreditCard> CreditCards => this.Set<CreditCard>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _ = builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
