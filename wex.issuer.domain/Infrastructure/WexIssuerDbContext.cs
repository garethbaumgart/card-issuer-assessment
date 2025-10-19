using Microsoft.EntityFrameworkCore;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Infrastructure;

public class WexIssuerDbContext(DbContextOptions<WexIssuerDbContext> options) : DbContext(options)
{
    public DbSet<Card> Cards { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WexIssuerDbContext).Assembly);
    }
}