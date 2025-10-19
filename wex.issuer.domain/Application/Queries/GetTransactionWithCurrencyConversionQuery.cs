using wex.issuer.domain.Application.Interfaces;

namespace wex.issuer.domain.Application.Queries;

public record GetTransactionWithCurrencyConversionQuery(Guid TransactionId, string TargetCurrency) 
    : IQuery<ConvertedTransactionResult?>;

public record ConvertedTransactionResult
{
    public Guid Id { get; init; }
    public Guid CardId { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset TransactionDate { get; init; }
    public decimal OriginalAmount { get; init; }
    public string OriginalCurrency { get; init; } = "USD";
    public decimal ExchangeRate { get; init; }
    public decimal ConvertedAmount { get; init; }
    public string TargetCurrency { get; init; } = string.Empty;
    public DateTimeOffset ExchangeRateDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}