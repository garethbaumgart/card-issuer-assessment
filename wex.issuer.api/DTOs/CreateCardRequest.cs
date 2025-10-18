using System.ComponentModel.DataAnnotations;

namespace wex.issuer.api.DTOs;

/// <summary>
/// Request DTO for creating a new card
/// </summary>
public record CreateCardRequest
{
    /// <summary>
    /// The credit limit for the card
    /// </summary>
    [Required(ErrorMessage = "Credit limit is required")]
    public decimal CreditLimit { get; init; }

    /// <summary>
    /// The currency for the card (optional, defaults to USD)
    /// </summary>
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-character code")]
    public string? Currency { get; init; } = "USD";
}