using Moq;
using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Commands;

public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockCardRepository = new Mock<ICardRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateTransactionCommandHandler(
            _mockTransactionRepository.Object, 
            _mockCardRepository.Object, 
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnTransaction()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = Card.Create(1000.00m, "USD");
        var command = new CreateTransactionCommand(
            cardId, 
            "Test Purchase", 
            DateTimeOffset.UtcNow.AddMinutes(-30), 
            99.99m);

        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync(card);
        _mockTransactionRepository.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction transaction) => transaction);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cardId, result.CardId);
        Assert.Equal("Test Purchase", result.Description);
        Assert.Equal(99.99m, result.PurchaseAmount);
        Assert.NotEqual(Guid.Empty, result.Id);

        // Verify repository calls
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
        _mockTransactionRepository.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCard_ShouldThrowDomainException()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var command = new CreateTransactionCommand(
            cardId, 
            "Test Purchase", 
            DateTimeOffset.UtcNow.AddMinutes(-30), 
            99.99m);

        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync((Card?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Card with ID {cardId} not found.", exception.Message);

        // Verify only card repository was called
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
        _mockTransactionRepository.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.Handle(null!, CancellationToken.None));
    }
}