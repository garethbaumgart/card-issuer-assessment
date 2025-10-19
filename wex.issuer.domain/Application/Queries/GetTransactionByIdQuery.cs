using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Query to get a transaction by its unique identifier
/// </summary>
public record GetTransactionByIdQuery(Guid TransactionId) : IQuery<Transaction?>
{
    /// <summary>
    /// The unique identifier of the transaction to retrieve
    /// </summary>
    public Guid TransactionId { get; } = TransactionId;
}