using Moq;
using wex.issuer.domain.Application.Queries;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Queries;

public class GetTransactionByIdQueryHandlerTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly GetTransactionByIdQueryHandler _handler;

    public GetTransactionByIdQueryHandlerTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _handler = new GetTransactionByIdQueryHandler(_mockTransactionRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTransaction_ShouldReturnTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var transaction = Transaction.Create(cardId, "Test Purchase", DateTimeOffset.UtcNow.AddMinutes(-30), 99.99m);
        var query = new GetTransactionByIdQuery(transactionId);

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CardId, result.CardId);
        Assert.Equal(transaction.Description, result.Description);
        Assert.Equal(transaction.PurchaseAmount, result.PurchaseAmount);

        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTransaction_ShouldReturnNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var query = new GetTransactionByIdQuery(transactionId);

        _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync((Transaction?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockTransactionRepository.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullQuery_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.Handle(null!, CancellationToken.None));
    }
}