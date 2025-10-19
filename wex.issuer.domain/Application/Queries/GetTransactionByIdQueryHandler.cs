using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Queries;

/// <summary>
/// Handler for getting a transaction by its ID
/// </summary>
public class GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository) 
    : IQueryHandler<GetTransactionByIdQuery, Transaction?>
{
    public async Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await transactionRepository.GetByIdAsync(request.TransactionId);
    }
}