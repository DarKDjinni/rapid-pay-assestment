namespace Distillery.RapidPay.Infrastructure.Persistance.Configurations;

using Distillery.RapidPay.Domain.Entities;
using Distillery.RapidPay.Infrastructure.Persistance.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CreditCardConfiguration : BaseEntityConfiguration<CreditCard>
{
    public override void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        base.Configure(builder);

        _ = builder.HasKey(x => x.CardNumber);

        _ = builder.Property(x => x.CardNumber).HasMaxLength(4000);
        _ = builder.Property(x => x.CreditLine).HasPrecision(16, 2);
        _ = builder.Property(x => x.CreditSpent).HasPrecision(16, 2);
        _ = builder.Property(x => x.Month);
        _ = builder.Property(x => x.OwnerName).HasMaxLength(256);
        _ = builder.Property(x => x.Year);
        _ = builder.Property(x => x.Ccv).HasMaxLength(1000);

        _ = builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
    }
}
