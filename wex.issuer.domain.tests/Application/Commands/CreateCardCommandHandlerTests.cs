using Moq;
using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.tests.Application.Commands;

public class CreateCardCommandHandlerTests
{
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateCardCommandHandler _handler;

    public CreateCardCommandHandlerTests()
    {
        _mockCardRepository = new Mock<ICardRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateCardCommandHandler(_mockCardRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnCard()
    {
        // Arrange
        var command = new CreateCardCommand(1000.50m, "USD");

        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card card) => card);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000.50m, result.CreditLimit);
        Assert.Equal("USD", result.Currency);
        Assert.NotEqual(Guid.Empty, result.Id);

        // Verify repository calls
        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryAndUnitOfWorkInCorrectOrder()
    {
        // Arrange
        var command = new CreateCardCommand(500m, "EUR");
        var callOrder = new List<string>();

        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ReturnsAsync((Card card) => card)
            .Callback(() => callOrder.Add("CreateAsync"));

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1)
            .Callback(() => callOrder.Add("SaveChangesAsync"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "CreateAsync", "SaveChangesAsync" }, callOrder);
    }

    [Fact]
    public async Task Handle_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldNotCallUnitOfWork()
    {
        // Arrange
        var command = new CreateCardCommand(1000m, "USD");

        _mockCardRepository.Setup(r => r.CreateAsync(It.IsAny<Card>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));

        _mockCardRepository.Verify(r => r.CreateAsync(It.IsAny<Card>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}