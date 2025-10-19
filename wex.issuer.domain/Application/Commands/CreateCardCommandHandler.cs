using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Commands;

/// <summary>
/// Handler for creating a new card
/// </summary>
public class CreateCardCommandHandler(ICardRepository cardRepository, IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateCardCommand, Card>
{
    public async Task<Card> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create the card using domain factory method
        var card = Card.Create(request.CreditLimit, request.Currency);

        // Persist the card
        await cardRepository.CreateAsync(card);
        await unitOfWork.SaveChangesAsync();

        return card;
    }
}