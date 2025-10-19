using Moq;
using wex.issuer.domain.Application.Queries;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Queries;

public class GetCardByIdQueryHandlerTests
{
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly GetCardByIdQueryHandler _handler;

    public GetCardByIdQueryHandlerTests()
    {
        _mockCardRepository = new Mock<ICardRepository>();
        _handler = new GetCardByIdQueryHandler(_mockCardRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnCard()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = Card.Create(1000m, "USD");
        var query = new GetCardByIdQuery(cardId);

        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync(card);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(card.Id, result.Id);
        Assert.Equal(card.CreditLimit, result.CreditLimit);
        Assert.Equal(card.Currency, result.Currency);

        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var query = new GetCardByIdQuery(cardId);

        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullQuery_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldCallRepository()
    {
        // Arrange
        var cardId = Guid.Empty;
        var query = new GetCardByIdQuery(cardId);

        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }
}