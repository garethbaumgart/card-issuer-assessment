using System.ComponentModel.DataAnnotations;

namespace wex.issuer.api.DTOs;

/// <summary>
/// Request DTO for creating a new transaction
/// </summary>
public record CreateTransactionRequest
{
    /// <summary>
    /// The card ID this transaction belongs to
    /// </summary>
    [Required(ErrorMessage = "Card ID is required")]
    public Guid CardId { get; init; }

    /// <summary>
    /// Transaction description (max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 50 characters")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// When the transaction occurred
    /// </summary>
    [Required(ErrorMessage = "Transaction date is required")]
    public DateTimeOffset TransactionDate { get; init; }

    /// <summary>
    /// Purchase amount in USD
    /// </summary>
    [Required(ErrorMessage = "Purchase amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase amount must be greater than zero")]
    public decimal PurchaseAmount { get; init; }
}