using wex.issuer.domain.Application.Interfaces;
using wex.issuer.domain.Exceptions;
using wex.issuer.domain.External;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain.Application.Queries;

public class GetTransactionWithCurrencyConversionQueryHandler(
    ITransactionRepository transactionRepository,
    ITreasuryApiService treasuryApiService) 
    : IQueryHandler<GetTransactionWithCurrencyConversionQuery, ConvertedTransactionResult?>
{
    public async Task<ConvertedTransactionResult?> Handle(GetTransactionWithCurrencyConversionQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.TargetCurrency))
            throw new DomainException("Target currency is required");

        var transaction = await transactionRepository.GetByIdAsync(request.TransactionId);
        if (transaction == null)
            return null;

        if (request.TargetCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase))
        {
            return new ConvertedTransactionResult
            {
                Id = transaction.Id,
                CardId = transaction.CardId,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                OriginalAmount = transaction.PurchaseAmount,
                OriginalCurrency = "USD",
                ExchangeRate = 1.0m,
                ConvertedAmount = transaction.PurchaseAmount,
                TargetCurrency = "USD",
                ExchangeRateDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt
            };
        }

        var exchangeRate = await treasuryApiService.GetExchangeRateAsync(
            request.TargetCurrency, 
            transaction.TransactionDate, 
            cancellationToken);

        if (exchangeRate == null)
        {
            throw new DomainException(
                $"Unable to convert to {request.TargetCurrency}. No exchange rate available " +
                $"for the purchase date {transaction.TransactionDate:yyyy-MM-dd} within the last 6 months.");
        }

        var convertedAmount = Math.Round(transaction.PurchaseAmount * exchangeRate.Rate, 2);

        return new ConvertedTransactionResult
        {
            Id = transaction.Id,
            CardId = transaction.CardId,
            Description = transaction.Description,
            TransactionDate = transaction.TransactionDate,
            OriginalAmount = transaction.PurchaseAmount,
            OriginalCurrency = "USD",
            ExchangeRate = exchangeRate.Rate,
            ConvertedAmount = convertedAmount,
            TargetCurrency = exchangeRate.Currency,
            ExchangeRateDate = exchangeRate.Date,
            CreatedAt = transaction.CreatedAt
        };
    }
}