namespace wex.issuer.api.DTOs;

/// <summary>
/// Response DTO for transaction operations
/// </summary>
public record TransactionResponse
{
    /// <summary>
    /// The unique identifier of the transaction
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The card ID this transaction belongs to
    /// </summary>
    public Guid CardId { get; init; }

    /// <summary>
    /// Transaction description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// When the transaction occurred
    /// </summary>
    public DateTimeOffset TransactionDate { get; init; }

    /// <summary>
    /// Purchase amount in USD
    /// </summary>
    public decimal PurchaseAmount { get; init; }

    /// <summary>
    /// When the transaction was created in the system
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}