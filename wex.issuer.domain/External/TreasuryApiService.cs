using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using wex.issuer.domain.External.Models;

namespace wex.issuer.domain.External;

public class TreasuryApiService(HttpClient httpClient, ILogger<TreasuryApiService> logger) : ITreasuryApiService
{
    private const string BaseUrl = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange";

    public async Task<ExchangeRate?> GetExchangeRateAsync(string currency, DateTimeOffset referenceDate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        try
        {
            var treasuryResponse = await FetchExchangeRatesFromApi(currency, referenceDate, cancellationToken);
            
            if (treasuryResponse?.Data == null || !treasuryResponse.Data.Any())
            {
                logger.LogWarning("No exchange rate data found for currency {Currency} in the specified date range", currency);
                return null;
            }

            var matchingRate = FindBestMatchingRate(treasuryResponse.Data, currency, referenceDate);

            if (matchingRate != null) return CreateExchangeRateFromData(matchingRate, currency);
            
            logger.LogWarning("No matching exchange rate found for currency {Currency} on or before {ReferenceDate}", 
                currency, referenceDate);
            return null;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while retrieving exchange rate for currency {Currency}", currency);
            throw;
        }
    }

    internal async Task<TreasuryExchangeRateResponse?> FetchExchangeRatesFromApi(string currency, DateTimeOffset referenceDate, CancellationToken cancellationToken)
    {
        var url = BuildApiUrl(currency, referenceDate);
        
        logger.LogDebug("Calling Treasury API for currency {Currency} up to {ReferenceDate}", 
            currency, referenceDate.ToString("yyyy-MM-dd"));

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TreasuryExchangeRateResponse>(jsonContent);
    }

    internal string BuildApiUrl(string currency, DateTimeOffset referenceDate)
    {
        var endDate = referenceDate.ToString("yyyy-MM-dd");

        return $"{BaseUrl}?fields=country,currency,exchange_rate,record_date" +
               $"&filter=currency:eq:{currency},record_date:lte:{endDate}" +
               $"&sort=-record_date&page[size]=50";
    }

    internal TreasuryExchangeRateData? FindBestMatchingRate(IList<TreasuryExchangeRateData> data, string currency, DateTimeOffset referenceDate)
    {
        // API already filters by currency and sorts by date descending,
        // just validating date constraint as a safety check
        return data
            .FirstOrDefault(d => DateTimeOffset.TryParse(d.RecordDate, out var recordDate) && recordDate <= referenceDate);
    }

    internal ExchangeRate? CreateExchangeRateFromData(TreasuryExchangeRateData data, string currency)
    {
        if (!decimal.TryParse(data.ExchangeRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
        {
            logger.LogError("Invalid exchange rate format: {ExchangeRate} for currency {Currency}", 
                data.ExchangeRate, currency);
            return null;
        }

        var exchangeRateDate = DateTimeOffset.Parse(data.RecordDate);
        var exchangeRate = ExchangeRate.Create(currency, rate, exchangeRateDate);

        logger.LogInformation("Found exchange rate {Rate} for currency {Currency} on {Date}", 
            rate, currency, exchangeRateDate);

        return exchangeRate;
    }
}