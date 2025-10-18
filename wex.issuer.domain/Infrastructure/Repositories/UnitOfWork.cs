using Microsoft.EntityFrameworkCore.Storage;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Infrastructure.Repositories;

public class UnitOfWork(WexIssuerDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}