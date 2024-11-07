namespace Distillery.RapidPay.Infrastructure.Persistance.Interceptors;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

public sealed class RecordOwnerInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        this.UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries<IEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && !string.IsNullOrWhiteSpace(currentUserService.UserId))
            {
                entry.Entity.UserId = currentUserService.UserId;
            }
        }
    }
}
