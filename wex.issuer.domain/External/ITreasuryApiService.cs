using wex.issuer.domain.External.Models;

namespace wex.issuer.domain.External;

public interface ITreasuryApiService
{
    Task<ExchangeRate?> GetExchangeRateAsync(string currency, DateTimeOffset referenceDate, CancellationToken cancellationToken = default);
}