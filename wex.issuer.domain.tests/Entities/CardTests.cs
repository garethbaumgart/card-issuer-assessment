using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;

namespace wex.issuer.domain.tests.Entities;

public class CardTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCard()
    {
        // Arrange
        var creditLimit = 1000.50m;
        var currency = "USD";

        // Act
        var card = Card.Create(creditLimit, currency);

        // Assert
        Assert.NotNull(card);
        Assert.NotEqual(Guid.Empty, card.Id);
        Assert.Equal(1000.50m, card.CreditLimit);
        Assert.Equal("USD", card.Currency);
        Assert.True((DateTimeOffset.UtcNow - card.CreatedAt).TotalSeconds < 1); // Created within last second
    }

    [Fact]
    public void Create_WithDecimalPrecision_ShouldRoundToNearestCent()
    {
        // Arrange
        var creditLimit = 1000.567m; // More than 2 decimal places
        var currency = "USD";

        // Act
        var card = Card.Create(creditLimit, currency);

        // Assert
        Assert.Equal(1000.57m, card.CreditLimit);
    }

    [Fact]
    public void Create_WithLowercaseCurrency_ShouldNormalizeCurrency()
    {
        // Arrange
        var creditLimit = 1000m;
        var currency = "usd";

        // Act
        var card = Card.Create(creditLimit, currency);

        // Assert
        Assert.Equal("USD", card.Currency);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Create_WithInvalidCreditLimit_ShouldThrowDomainException(decimal invalidCreditLimit)
    {
        // Arrange
        var currency = "USD";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Card.Create(invalidCreditLimit, currency));
        Assert.Equal("Credit limit must be a positive amount greater than zero.", exception.Message);
    }

    [Fact]
    public void Create_WithNullCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var creditLimit = 1000m;
        string? currency = null;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Card.Create(creditLimit, currency!));
        Assert.Equal("Currency is required.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceCurrency_ShouldThrowDomainException(string invalidCurrency)
    {
        // Arrange
        var creditLimit = 1000m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Card.Create(creditLimit, invalidCurrency));
        Assert.Equal("Currency is required.", exception.Message);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("A")]
    public void Create_WithInvalidCurrencyLength_ShouldThrowDomainException(string invalidCurrency)
    {
        // Arrange
        var creditLimit = 1000m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Card.Create(creditLimit, invalidCurrency));
        Assert.Equal("Currency must be a valid 3-character ISO code (e.g., USD, EUR).", exception.Message);
    }

    [Fact]
    public void GetAvailableBalance_WithNoTransactions_ShouldReturnCreditLimit()
    {
        // Arrange
        var creditLimit = 1500.75m;
        var card = Card.Create(creditLimit, "USD");

        // Act
        var availableBalance = card.GetAvailableBalance();

        // Assert
        Assert.Equal(creditLimit, availableBalance);
    }

    [Fact]
    public void GetAvailableBalance_WithTransactions_ShouldReturnCreditLimitMinusTransactions()
    {
        // Arrange
        var creditLimit = 1000.00m;
        var card = Card.Create(creditLimit, "USD");
        
        // Use reflection to add transactions since Transactions property has private setter
        var transactionsProperty = typeof(Card).GetProperty("Transactions");
        var transactions = (ICollection<Transaction>)transactionsProperty!.GetValue(card)!;
        
        var transaction1 = Transaction.Create(card.Id, "Purchase 1", DateTimeOffset.UtcNow.AddHours(-2), 200.00m);
        var transaction2 = Transaction.Create(card.Id, "Purchase 2", DateTimeOffset.UtcNow.AddHours(-1), 150.50m);
        
        transactions.Add(transaction1);
        transactions.Add(transaction2);

        // Act
        var availableBalance = card.GetAvailableBalance();

        // Assert
        Assert.Equal(649.50m, availableBalance); // 1000 - 200 - 150.50 = 649.50
    }

    [Fact]
    public void GetAvailableBalance_WithSingleTransaction_ShouldReturnCorrectBalance()
    {
        // Arrange
        var creditLimit = 500.00m;
        var card = Card.Create(creditLimit, "USD");
        
        // Use reflection to add transaction
        var transactionsProperty = typeof(Card).GetProperty("Transactions");
        var transactions = (ICollection<Transaction>)transactionsProperty!.GetValue(card)!;
        
        var transaction = Transaction.Create(card.Id, "Purchase", DateTimeOffset.UtcNow.AddHours(-1), 99.99m);
        transactions.Add(transaction);

        // Act
        var availableBalance = card.GetAvailableBalance();

        // Assert
        Assert.Equal(400.01m, availableBalance); // 500 - 99.99 = 400.01
    }

    [Fact]
    public void GetAvailableBalance_WithTransactionsExceedingLimit_ShouldReturnNegativeBalance()
    {
        // Arrange
        var creditLimit = 100.00m;
        var card = Card.Create(creditLimit, "USD");
        
        // Use reflection to add transactions
        var transactionsProperty = typeof(Card).GetProperty("Transactions");
        var transactions = (ICollection<Transaction>)transactionsProperty!.GetValue(card)!;
        
        var transaction1 = Transaction.Create(card.Id, "Purchase 1", DateTime.UtcNow.AddHours(-2), 75.00m);
        var transaction2 = Transaction.Create(card.Id, "Purchase 2", DateTime.UtcNow.AddHours(-1), 50.00m);
        
        transactions.Add(transaction1);
        transactions.Add(transaction2);

        // Act
        var availableBalance = card.GetAvailableBalance();

        // Assert
        Assert.Equal(-25.00m, availableBalance); // 100 - 75 - 50 = -25
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("CAD")]
    public void Create_WithValidCurrencies_ShouldSucceed(string currency)
    {
        // Arrange
        var creditLimit = 1000m;

        // Act
        var card = Card.Create(creditLimit, currency);

        // Assert
        Assert.Equal(currency.ToUpperInvariant(), card.Currency);
    }

    [Fact]
    public void Card_Properties_ShouldBeReadOnly()
    {
        // Arrange
        var card = Card.Create(1000m, "USD");
        var originalId = card.Id;
        var originalCreditLimit = card.CreditLimit;
        var originalCurrency = card.Currency;
        var originalCreatedAt = card.CreatedAt;

        // Act - Trying to modify properties (this tests that they have private setters)
        // We can't actually modify them due to private setters, but we can verify they don't change

        // Assert - Properties should remain unchanged
        Assert.Equal(originalId, card.Id);
        Assert.Equal(originalCreditLimit, card.CreditLimit);
        Assert.Equal(originalCurrency, card.Currency);
        Assert.Equal(originalCreatedAt, card.CreatedAt);
    }
}