using wex.issuer.domain.Exceptions;

namespace wex.issuer.domain.Entities;

public class Card
{
    public Guid Id { get; private set; }
    public decimal CreditLimit { get; private set; }
    public string Currency { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Navigation property for EF Core
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    // Private constructor for EF Core
    private Card()
    {
        Currency = string.Empty; // Required for nullable reference types
    }

    // Private constructor for domain use
    private Card(Guid id, decimal creditLimit, string currency, DateTimeOffset createdAt)
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
            createdAt: DateTimeOffset.UtcNow
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
    /// Get available balance by subtracting total transactions from credit limit
    /// </summary>
    /// <returns>Available balance (Credit Limit - Total Transactions)</returns>
    public decimal GetAvailableBalance()
    {
        var totalTransactions = Transactions?.Sum(t => t.PurchaseAmount) ?? 0;
        return CreditLimit - totalTransactions;
    }

    /// <summary>
    /// Get available balance with provided transactions collection
    /// Useful when transactions are loaded separately from the repository
    /// </summary>
    /// <param name="transactions">Collection of transactions for this card</param>
    /// <returns>Available balance (Credit Limit - Total Transactions)</returns>
    public decimal GetAvailableBalance(IEnumerable<Transaction> transactions)
    {
        var totalTransactions = transactions?.Sum(t => t.PurchaseAmount) ?? 0;
        return CreditLimit - totalTransactions;
    }
}