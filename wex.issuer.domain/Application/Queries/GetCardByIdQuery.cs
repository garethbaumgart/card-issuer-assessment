using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Query to get a card by its unique identifier
/// </summary>
public record GetCardByIdQuery(Guid CardId) : IQuery<Card?>
{
    /// <summary>
    /// The unique identifier of the card to retrieve
    /// </summary>
    public Guid CardId { get; } = CardId;
}