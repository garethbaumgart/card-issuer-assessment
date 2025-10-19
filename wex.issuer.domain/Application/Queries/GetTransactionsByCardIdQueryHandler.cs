using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Handler for getting transactions by card ID with pagination
/// </summary>
public class GetTransactionsByCardIdQueryHandler(ITransactionRepository transactionRepository) 
    : IQueryHandler<GetTransactionsByCardIdQuery, IEnumerable<Transaction>>
{
    public async Task<IEnumerable<Transaction>> Handle(GetTransactionsByCardIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await transactionRepository.GetByCardIdAsync(request.CardId, request.Skip, request.Take);
    }
}