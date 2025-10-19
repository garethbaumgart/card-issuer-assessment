using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;

namespace wex.issuer.domain.Application.Commands;

/// <summary>
/// Command to create a new card with validation
/// </summary>
public record CreateCardCommand(decimal CreditLimit, string Currency = "USD") : ICommand<Card>
{
    /// <summary>
    /// The credit limit for the card
    /// </summary>
    public decimal CreditLimit { get; } = CreditLimit;

    /// <summary>
    /// The currency for the card (defaults to USD)
    /// </summary>
    public string Currency { get; } = Currency;
}