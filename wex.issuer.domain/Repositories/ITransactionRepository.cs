using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Repositories;

public interface ITransactionRepository
{
    /// <summary>
    /// Creates a new transaction in the repository
    /// </summary>
    /// <param name="transaction">The transaction entity to create</param>
    /// <returns>The created transaction entity</returns>
    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>
    /// Retrieves a transaction by its unique identifier
    /// </summary>
    /// <param name="id">The transaction's unique identifier</param>
    /// <returns>The transaction if found, null otherwise</returns>
    Task<Transaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all transactions for a specific card
    /// </summary>
    /// <param name="cardId">The card's unique identifier</param>
    /// <returns>Collection of transactions for the card</returns>
    Task<IEnumerable<Transaction>> GetByCardIdAsync(Guid cardId);

    /// <summary>
    /// Retrieves transactions for a specific card with pagination
    /// </summary>
    /// <param name="cardId">The card's unique identifier</param>
    /// <param name="skip">Number of transactions to skip</param>
    /// <param name="take">Number of transactions to take</param>
    /// <returns>Collection of transactions for the card</returns>
    Task<IEnumerable<Transaction>> GetByCardIdAsync(Guid cardId, int skip, int take);

    /// <summary>
    /// Gets the total count of transactions for a specific card
    /// </summary>
    /// <param name="cardId">The card's unique identifier</param>
    /// <returns>Total number of transactions for the card</returns>
    Task<int> GetCountByCardIdAsync(Guid cardId);
}