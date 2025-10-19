namespace wex.issuer.api.DTOs;

/// <summary>
/// Response DTO for transaction with currency conversion
/// </summary>
public record ConvertedTransactionResponse
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
    /// Original purchase amount in USD
    /// </summary>
    public decimal OriginalAmount { get; init; }

    /// <summary>
    /// Original currency (always USD)
    /// </summary>
    public string OriginalCurrency { get; init; } = "USD";

    /// <summary>
    /// Exchange rate used for conversion
    /// </summary>
    public decimal ExchangeRate { get; init; }

    /// <summary>
    /// Converted amount in target currency
    /// </summary>
    public decimal ConvertedAmount { get; init; }

    /// <summary>
    /// Target currency for conversion
    /// </summary>
    public string TargetCurrency { get; init; } = string.Empty;

    /// <summary>
    /// Date of the exchange rate used
    /// </summary>
    public DateTimeOffset ExchangeRateDate { get; init; }

    /// <summary>
    /// When the transaction was created in the system
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}