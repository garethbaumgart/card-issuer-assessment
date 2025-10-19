using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Application.Commands;

/// <summary>
/// Command to create a new transaction with validation
/// </summary>
public record CreateTransactionCommand(Guid CardId, string Description, DateTimeOffset TransactionDate, decimal PurchaseAmount) : ICommand<Transaction>
{
    /// <summary>
    /// The card this transaction belongs to
    /// </summary>
    public Guid CardId { get; } = CardId;

    /// <summary>
    /// Transaction description (max 50 characters)
    /// </summary>
    public string Description { get; } = Description;

    /// <summary>
    /// When the transaction occurred
    /// </summary>
    public DateTimeOffset TransactionDate { get; } = TransactionDate;

    /// <summary>
    /// Purchase amount in USD
    /// </summary>
    public decimal PurchaseAmount { get; } = PurchaseAmount;
}