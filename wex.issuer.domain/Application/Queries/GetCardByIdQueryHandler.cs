using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Handler for getting a card by its ID
/// </summary>
public class GetCardByIdQueryHandler(ICardRepository cardRepository) : IQueryHandler<GetCardByIdQuery, Card?>
{
    public async Task<Card?> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await cardRepository.GetByIdAsync(request.CardId);
    }
}