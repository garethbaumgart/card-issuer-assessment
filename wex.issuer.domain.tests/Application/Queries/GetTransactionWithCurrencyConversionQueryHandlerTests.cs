using Moq;
using wex.issuer.domain.Application.Queries;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.External;
using wex.issuer.domain.External.Models;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Queries;

public class GetTransactionWithCurrencyConversionQueryHandlerTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ITreasuryApiService> _mockTreasuryApiService;
    private readonly GetTransactionWithCurrencyConversionQueryHandler _handler;

    public GetTransactionWithCurrencyConversionQueryHandlerTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockTreasuryApiService = new Mock<ITreasuryApiService>();
        _handler = new GetTransactionWithCurrencyConversionQueryHandler(
            _mockTransactionRepository.Object,
            _mockTreasuryApiService.Object);
    }

    [Fact]
    public async Task Handle_WithValidTransactionAndCurrency_ShouldReturnConvertedTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var transaction = Transaction.Create(cardId, "Test Purchase", DateTimeOffset.UtcNow.AddDays(-10), 100.00m);
        var query = new GetTransactionWithCurrencyConversionQuery(transactionId, "Euro");
        var exchangeRate = ExchangeRate.Create("EURO", 0.85m, DateTimeOffset.UtcNow.AddDays(-15));

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        _mockTreasuryApiService.Setup(s => s.GetExchangeRateAsync("Euro", transaction.TransactionDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CardId, result.CardId);
        Assert.Equal(transaction.Description, result.Description);
        Assert.Equal(transaction.TransactionDate, result.TransactionDate);
        Assert.Equal(100.00m, result.OriginalAmount);
        Assert.Equal("USD", result.OriginalCurrency);
        Assert.Equal(0.85m, result.ExchangeRate);
        Assert.Equal(85.00m, result.ConvertedAmount);
        Assert.Equal("EURO", result.TargetCurrency);
        Assert.Equal(exchangeRate.Date, result.ExchangeRateDate);

        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        _mockTreasuryApiService.Verify(s => s.GetExchangeRateAsync("Euro", transaction.TransactionDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUSDCurrency_ShouldReturnOriginalAmountWithoutApiCall()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var transaction = Transaction.Create(cardId, "Test Purchase", DateTimeOffset.UtcNow.AddDays(-10), 100.00m);
        var query = new GetTransactionWithCurrencyConversionQuery(transactionId, "USD");

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(100.00m, result.OriginalAmount);
        Assert.Equal("USD", result.OriginalCurrency);
        Assert.Equal(1.0m, result.ExchangeRate);
        Assert.Equal(100.00m, result.ConvertedAmount);
        Assert.Equal("USD", result.TargetCurrency);
        Assert.Equal(transaction.TransactionDate, result.ExchangeRateDate);

        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        _mockTreasuryApiService.Verify(s => s.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentTransaction_ShouldReturnNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionWithCurrencyConversionQuery(transactionId, "Euro");

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync((Transaction?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        _mockTreasuryApiService.Verify(s => s.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNoExchangeRateAvailable_ShouldThrowDomainException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var transaction = Transaction.Create(cardId, "Test Purchase", DateTimeOffset.UtcNow.AddDays(-10), 100.00m);
        var query = new GetTransactionWithCurrencyConversionQuery(transactionId, "Euro");

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        _mockTreasuryApiService.Setup(s => s.GetExchangeRateAsync("Euro", transaction.TransactionDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExchangeRate?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => 
            _handler.Handle(query, CancellationToken.None));

        Assert.Contains("Unable to convert to Euro", exception.Message);
        Assert.Contains("No exchange rate available", exception.Message);
        Assert.Contains("within the last 6 months", exception.Message);

        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        _mockTreasuryApiService.Verify(s => s.GetExchangeRateAsync("Euro", transaction.TransactionDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullOrEmptyTargetCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _handler.Handle(new GetTransactionWithCurrencyConversionQuery(transactionId, null!), CancellationToken.None));

        await Assert.ThrowsAsync<DomainException>(() => 
            _handler.Handle(new GetTransactionWithCurrencyConversionQuery(transactionId, ""), CancellationToken.None));

        await Assert.ThrowsAsync<DomainException>(() => 
            _handler.Handle(new GetTransactionWithCurrencyConversionQuery(transactionId, "   "), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNullQuery_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithRoundingRequired_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var transaction = Transaction.Create(cardId, "Test Purchase", DateTimeOffset.UtcNow.AddDays(-10), 100.33m);
        var query = new GetTransactionWithCurrencyConversionQuery(transactionId, "Euro");
        var exchangeRate = ExchangeRate.Create("EURO", 0.8567m, DateTimeOffset.UtcNow.AddDays(-15));

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        _mockTreasuryApiService.Setup(s => s.GetExchangeRateAsync("Euro", transaction.TransactionDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.33m, result.OriginalAmount);
        Assert.Equal(0.8567m, result.ExchangeRate);
        Assert.Equal(85.95m, result.ConvertedAmount); // 100.33 * 0.8567 = 85.952611 -> rounded to 85.95
    }
}