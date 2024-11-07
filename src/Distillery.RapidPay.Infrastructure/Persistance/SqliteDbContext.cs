namespace Distillery.RapidPay.Infrastructure.Persistance;

using Distillery.RapidPay.Infrastructure.Persistance.Common;
using Microsoft.EntityFrameworkCore;

public sealed class SqliteDbContext(DbContextOptions options) : BaseDbContext(options)
{
}
