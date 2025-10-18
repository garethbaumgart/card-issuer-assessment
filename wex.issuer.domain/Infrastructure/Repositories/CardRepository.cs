using Microsoft.EntityFrameworkCore;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Infrastructure.Repositories;

public class CardRepository(WexIssuerDbContext context) : ICardRepository
{
    public async Task<Card> CreateAsync(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        var entry = await context.Cards.AddAsync(card);
        return entry.Entity;
    }

    public async Task<Card?> GetByIdAsync(Guid id)
    {
        return await context.Cards
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}