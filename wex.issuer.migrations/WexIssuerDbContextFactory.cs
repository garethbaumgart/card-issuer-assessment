using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using wex.issuer.domain.Infrastructure;

namespace wex.issuer.migrations;

public class WexIssuerDbContextFactory : IDesignTimeDbContextFactory<WexIssuerDbContext>
{
    public WexIssuerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WexIssuerDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=wex_issuer;Username=admin;Password=password",
            b => b.MigrationsAssembly("wex.issuer.migrations"));

        return new WexIssuerDbContext(optionsBuilder.Options);
    }
}