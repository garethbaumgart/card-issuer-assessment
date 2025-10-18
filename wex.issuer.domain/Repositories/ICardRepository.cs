using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Repositories;

public interface ICardRepository
{
    /// <summary>
    /// Creates a new card in the repository
    /// </summary>
    /// <param name="card">The card entity to create</param>
    /// <returns>The created card entity</returns>
    Task<Card> CreateAsync(Card card);

    /// <summary>
    /// Retrieves a card by its unique identifier
    /// </summary>
    /// <param name="id">The card's unique identifier</param>
    /// <returns>The card if found, null otherwise</returns>
    Task<Card?> GetByIdAsync(Guid id);
}