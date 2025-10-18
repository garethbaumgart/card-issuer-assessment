using Moq;
using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Application.Services;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Services;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CardService _cardService;

    public CardServiceTests()
    {
        _mockCardRepository = new Mock<ICardRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _cardService = new CardService(_mockCardRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateCardAsync_WithValidCommand_ShouldReturnCard()
    {
        // Arrange
        var command = new CreateCardCommand(1000.50m, "USD");

        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card card) => card);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _cardService.CreateCardAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000.50m, result.CreditLimit);
        Assert.Equal("USD", result.Currency);
        Assert.NotEqual(Guid.Empty, result.Id);
        
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCardAsync_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Arrange
        CreateCardCommand? command = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _cardService.CreateCardAsync(command!));
        
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCardAsync_WithInvalidCreditLimit_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateCardCommand(-100m, "USD"); // Invalid credit limit

        // Act & Assert
        await Assert.ThrowsAsync<wex.issuer.domain.Exceptions.DomainException>(
            () => _cardService.CreateCardAsync(command));
        
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCardAsync_WithInvalidCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateCardCommand(1000m, "INVALID"); // Invalid currency length

        // Act & Assert
        await Assert.ThrowsAsync<wex.issuer.domain.Exceptions.DomainException>(
            () => _cardService.CreateCardAsync(command));
        
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCardAsync_RepositoryThrowsException_ShouldNotCallUnitOfWork()
    {
        // Arrange
        var command = new CreateCardCommand(1000m, "USD");

        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _cardService.CreateCardAsync(command));
        
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetCardByIdAsync_WithValidId_ShouldReturnCard()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var expectedCard = Card.Create(1000m, "USD");
        
        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync(expectedCard);

        // Act
        var result = await _cardService.GetCardByIdAsync(cardId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCard.Id, result.Id);
        Assert.Equal(expectedCard.CreditLimit, result.CreditLimit);
        Assert.Equal(expectedCard.Currency, result.Currency);
        
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }

    [Fact]
    public async Task GetCardByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        
        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _cardService.GetCardByIdAsync(cardId);

        // Assert
        Assert.Null(result);
        
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }

    [Fact]
    public async Task GetCardByIdAsync_WithEmptyGuid_ShouldCallRepository()
    {
        // Arrange
        var cardId = Guid.Empty;
        
        _mockCardRepository.Setup(r => r.GetByIdAsync(cardId))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _cardService.GetCardByIdAsync(cardId);

        // Assert
        Assert.Null(result);
        
        _mockCardRepository.Verify(r => r.GetByIdAsync(cardId), Times.Once);
    }

    [Fact]
    public async Task CreateCardAsync_ShouldCallRepositoryAndUnitOfWorkInCorrectOrder()
    {
        // Arrange
        var command = new CreateCardCommand(1000m, "USD");

        var callOrder = new List<string>();
        
        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .Callback(() => callOrder.Add("CreateAsync"))
            .ReturnsAsync((Card card) => card);
            
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .Callback(() => callOrder.Add("SaveChangesAsync"))
            .ReturnsAsync(1);

        // Act
        await _cardService.CreateCardAsync(command);

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("CreateAsync", callOrder[0]);
        Assert.Equal("SaveChangesAsync", callOrder[1]);
    }
}