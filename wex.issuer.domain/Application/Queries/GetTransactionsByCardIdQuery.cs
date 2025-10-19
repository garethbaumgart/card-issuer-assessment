using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Query to get transactions for a specific card with pagination
/// </summary>
public record GetTransactionsByCardIdQuery(Guid CardId, int Skip = 0, int Take = 50) : IQuery<IEnumerable<Transaction>>
{
    /// <summary>
    /// The unique identifier of the card
    /// </summary>
    public Guid CardId { get; } = CardId;

    /// <summary>
    /// Number of transactions to skip (for pagination)
    /// </summary>
    public int Skip { get; } = Skip;

    /// <summary>
    /// Number of transactions to take (for pagination)
    /// </summary>
    public int Take { get; } = Take;
}