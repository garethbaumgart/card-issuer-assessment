using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Services;

/// <summary>
/// Application service for card operations following DDD patterns
/// </summary>
public class CardService(ICardRepository cardRepository, IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Creates a new card using domain factory method and unit of work
    /// </summary>
    /// <param name="command">The create card command</param>
    /// <returns>The created card entity</returns>
    /// <exception cref="ArgumentNullException">Thrown when command is null</exception>
    /// <exception cref="DomainException">Thrown when domain validation fails</exception>
    public async Task<Card> CreateCardAsync(CreateCardCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var card = Card.Create(command.CreditLimit, command.Currency);
        await cardRepository.CreateAsync(card);
        await unitOfWork.SaveChangesAsync();

        return card;
    }

    /// <summary>
    /// Retrieves a card by its unique identifier
    /// </summary>
    /// <param name="cardId">The card's unique identifier</param>
    /// <returns>The card if found, null otherwise</returns>
    public async Task<Card?> GetCardByIdAsync(Guid cardId)
    {
        return await cardRepository.GetByIdAsync(cardId);
    }
}