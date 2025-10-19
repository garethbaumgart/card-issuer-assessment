using Microsoft.EntityFrameworkCore;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Infrastructure.Repositories;

public class TransactionRepository(WexIssuerDbContext context) : ITransactionRepository
{
    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        var entry = await context.Transactions.AddAsync(transaction);
        return entry.Entity;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await context.Transactions
            .Include(t => t.Card)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByCardIdAsync(Guid cardId)
    {
        return await context.Transactions
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByCardIdAsync(Guid cardId, int skip, int take)
    {
        return await context.Transactions
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountByCardIdAsync(Guid cardId)
    {
        return await context.Transactions
            .CountAsync(t => t.CardId == cardId);
    }
}