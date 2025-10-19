using wex.issuer.domain.Application.Interfaces;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Query to get the available balance for a card
/// </summary>
public record GetAvailableBalanceQuery(Guid CardId) : IQuery<decimal>
{
    /// <summary>
    /// The unique identifier of the card
    /// </summary>
    public Guid CardId { get; } = CardId;
}