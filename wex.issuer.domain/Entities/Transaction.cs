using wex.issuer.domain.Exceptions;

namespace wex.issuer.domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid CardId { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public decimal PurchaseAmount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Navigation property for EF Core
    public Card Card { get; private set; } = null!;

    // Private constructor for EF Core
    private Transaction()
    {
        Description = string.Empty; // Required for nullable reference types
    }

    // Private constructor for domain use
    private Transaction(Guid id, Guid cardId, string description, DateTimeOffset transactionDate, 
        decimal purchaseAmount, DateTimeOffset createdAt)
    {
        Id = id;
        CardId = cardId;
        Description = description;
        TransactionDate = transactionDate;
        PurchaseAmount = purchaseAmount;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory method to create a new Transaction with domain validation
    /// </summary>
    /// <param name="cardId">The card this transaction belongs to</param>
    /// <param name="description">Transaction description (max 50 characters)</param>
    /// <param name="transactionDate">When the transaction occurred</param>
    /// <param name="purchaseAmount">Purchase amount in USD</param>
    /// <returns>A valid Transaction entity</returns>
    /// <exception cref="DomainException">Thrown when validation fails</exception>
    public static Transaction Create(Guid cardId, string description, DateTimeOffset transactionDate, decimal purchaseAmount)
    {
        // Domain validation
        ValidateCardId(cardId);
        ValidateDescription(description);
        ValidateTransactionDate(transactionDate);
        ValidatePurchaseAmount(purchaseAmount);

        return new Transaction(
            id: Guid.NewGuid(),
            cardId: cardId,
            description: description.Trim(),
            transactionDate: transactionDate,
            purchaseAmount: Math.Round(purchaseAmount, 2), // Round to nearest cent
            createdAt: DateTimeOffset.UtcNow
        );
    }

    private static void ValidateCardId(Guid cardId)
    {
        if (cardId == Guid.Empty)
        {
            throw new DomainException("Card ID is required.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Transaction description is required.");
        }

        if (description.Trim().Length > 50)
        {
            throw new DomainException("Transaction description must not exceed 50 characters.");
        }
    }

    private static void ValidateTransactionDate(DateTimeOffset transactionDate)
    {
        if (transactionDate == default(DateTimeOffset))
        {
            throw new DomainException("Transaction date is required.");
        }

        if (transactionDate > DateTimeOffset.UtcNow)
        {
            throw new DomainException("Transaction date cannot be in the future.");
        }
    }

    private static void ValidatePurchaseAmount(decimal purchaseAmount)
    {
        if (purchaseAmount <= 0)
        {
            throw new DomainException("Purchase amount must be a positive amount greater than zero.");
        }
    }
}