namespace wex.issuer.domain.External.Models;

public record ExchangeRate
{
    public string Currency { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public DateTimeOffset Date { get; init; }

    public static ExchangeRate Create(string currency, decimal rate, DateTimeOffset date)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));
        
        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(rate));

        return new ExchangeRate
        {
            Currency = currency.Trim().ToUpperInvariant(),
            Rate = rate,
            Date = date
        };
    }
}