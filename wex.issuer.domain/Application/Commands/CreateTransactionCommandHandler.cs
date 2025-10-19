using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Entities;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Commands;

/// <summary>
/// Handler for creating a new transaction
/// </summary>
public class CreateTransactionCommandHandler(ITransactionRepository transactionRepository, ICardRepository cardRepository, IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateTransactionCommand, Transaction>
{
    public async Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate that the card exists
        var card = await cardRepository.GetByIdAsync(request.CardId);
        if (card == null)
        {
            throw new DomainException($"Card with ID {request.CardId} not found.");
        }

        // Create the transaction using domain factory method
        var transaction = Transaction.Create(
            request.CardId,
            request.Description,
            request.TransactionDate,
            request.PurchaseAmount);

        // Persist the transaction
        await transactionRepository.CreateAsync(transaction);
        await unitOfWork.SaveChangesAsync();

        return transaction;
    }
}