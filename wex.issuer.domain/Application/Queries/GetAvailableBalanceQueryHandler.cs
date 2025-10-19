using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Handler for getting the available balance for a card
/// </summary>
public class GetAvailableBalanceQueryHandler(ICardRepository cardRepository, ITransactionRepository transactionRepository) 
    : IQueryHandler<GetAvailableBalanceQuery, decimal>
{
    public async Task<decimal> Handle(GetAvailableBalanceQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Get the card
        var card = await cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
        {
            throw new DomainException($"Card with ID {request.CardId} not found.");
        }

        // Get all transactions for the card
        var transactions = await transactionRepository.GetByCardIdAsync(request.CardId);

        // Calculate available balance
        var totalTransactions = transactions.Sum(t => t.PurchaseAmount);
        return card.CreditLimit - totalTransactions;
    }
}