namespace wex.issuer.api.DTOs;

/// <summary>
/// Response DTO for card operations
/// </summary>
public record CardResponse
{
    /// <summary>
    /// The unique identifier of the card
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The credit limit of the card
    /// </summary>
    public decimal CreditLimit { get; init; }

    /// <summary>
    /// The currency of the card
    /// </summary>
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// When the card was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// The available balance on the card
    /// </summary>
    public decimal AvailableBalance { get; init; }
}