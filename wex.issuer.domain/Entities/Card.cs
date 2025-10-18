using wex.issuer.domain.Exceptions;

namespace wex.issuer.domain.Entities;

public class Card
{
    public Guid Id { get; private set; }
    public decimal CreditLimit { get; private set; }
    public string Currency { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private Card()
    {
        Currency = string.Empty; // Required for nullable reference types
    }

    // Private constructor for domain use
    private Card(Guid id, decimal creditLimit, string currency, DateTime createdAt)
    {
        Id = id;
        CreditLimit = creditLimit;
        Currency = currency;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory method to create a new Card with domain validation
    /// </summary>
    /// <param name="creditLimit">Credit limit in USD</param>
    /// <param name="currency">Currency code</param>
    /// <returns>A valid Card entity</returns>
    /// <exception cref="DomainException">Thrown when validation fails</exception>
    public static Card Create(decimal creditLimit, string currency)
    {
        // Domain validation
        ValidateCreditLimit(creditLimit);
        ValidateCurrency(currency);

        return new Card(
            id: Guid.NewGuid(),
            creditLimit: Math.Round(creditLimit, 2), // Round to nearest cent
            currency: currency.ToUpperInvariant(), // Normalize currency
            createdAt: DateTime.UtcNow
        );
    }

    private static void ValidateCreditLimit(decimal creditLimit)
    {
        if (creditLimit <= 0)
        {
            throw new DomainException("Credit limit must be a positive amount greater than zero.");
        }
    }

    private static void ValidateCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        if (currency.Length != 3)
        {
            throw new DomainException("Currency must be a valid 3-character ISO code (e.g., USD, EUR).");
        }
    }

    /// <summary>
    /// Get available balance (for future use with transactions)
    /// Currently returns the full credit limit since no transactions exist
    /// </summary>
    public decimal GetAvailableBalance()
    {
        // For now, return full credit limit
        // Later: CreditLimit - total of all transactions
        return CreditLimit;
    }
}