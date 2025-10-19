using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;

namespace wex.issuer.domain.tests.Entities;

public class TransactionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnTransaction()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 99.99m;

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.NotNull(transaction);
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(cardId, transaction.CardId);
        Assert.Equal("Test Purchase", transaction.Description);
        Assert.Equal(transactionDate, transaction.TransactionDate);
        Assert.Equal(99.99m, transaction.PurchaseAmount);
        Assert.True((DateTimeOffset.UtcNow - transaction.CreatedAt).TotalSeconds < 1); // Created within last second
    }

    [Fact]
    public void Create_WithDecimalPrecision_ShouldRoundToNearestCent()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 99.996m; // More than 2 decimal places

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.Equal(100.00m, transaction.PurchaseAmount);
    }

    [Fact]
    public void Create_WithDescriptionHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "  Test Purchase  ";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.Equal("Test Purchase", transaction.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidDescription_ShouldThrowDomainException(string description)
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Transaction description is required.", exception.Message);
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        string description = null!;
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Transaction description is required.", exception.Message);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = new string('A', 51); // 51 characters, exceeds 50 limit
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Transaction description must not exceed 50 characters.", exception.Message);
    }

    [Fact]
    public void Create_WithDescriptionExactly50Characters_ShouldSucceed()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = new string('A', 50); // Exactly 50 characters
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(50, transaction.Description.Length);
    }

    [Fact]
    public void Create_WithEmptyCardId_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.Empty;
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Card ID is required.", exception.Message);
    }

    [Fact]
    public void Create_WithDefaultTransactionDate_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = default(DateTimeOffset);
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Transaction date is required.", exception.Message);
    }

    [Fact]
    public void Create_WithFutureTransactionDate_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddDays(1); // Future date
        var purchaseAmount = 50.00m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Transaction date cannot be in the future.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.00)]
    public void Create_WithInvalidPurchaseAmount_ShouldThrowDomainException(decimal purchaseAmount)
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Transaction.Create(cardId, description, transactionDate, purchaseAmount));
        Assert.Equal("Purchase amount must be a positive amount greater than zero.", exception.Message);
    }

    [Fact]
    public void Create_WithMinimumValidPurchaseAmount_ShouldSucceed()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Test Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 0.01m; // Minimum valid amount

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(0.01m, transaction.PurchaseAmount);
    }

    [Fact]
    public void Create_WithLargePurchaseAmount_ShouldSucceed()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var description = "Large Purchase";
        var transactionDate = DateTimeOffset.UtcNow.AddMinutes(-30);
        var purchaseAmount = 999999.99m; // Large amount

        // Act
        var transaction = Transaction.Create(cardId, description, transactionDate, purchaseAmount);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(999999.99m, transaction.PurchaseAmount);
    }
}