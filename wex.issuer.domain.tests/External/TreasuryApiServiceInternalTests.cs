using Microsoft.Extensions.Logging;
using Moq;
using wex.issuer.domain.External;
using wex.issuer.domain.External.Models;

namespace wex.issuer.domain.tests.External;

public class TreasuryApiServiceInternalTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<TreasuryApiService>> _mockLogger;
    private readonly HttpClient _httpClient;
    private readonly TreasuryApiService _service;

    public TreasuryApiServiceInternalTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockLogger = new Mock<ILogger<TreasuryApiService>>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new TreasuryApiService(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public void BuildApiUrl_ShouldConstructCorrectUrl()
    {
        // Arrange
        var referenceDate = DateTimeOffset.Parse("2024-06-15");

        // Act
        var url = _service.BuildApiUrl("EUR", referenceDate);

        // Assert
        Assert.Contains("rates_of_exchange", url);
        Assert.Contains("fields=country,currency,exchange_rate,record_date", url);
        Assert.Contains("currency:eq:EUR", url);
        Assert.Contains("record_date:lte:2024-06-15", url);
        Assert.Contains("sort=-record_date", url);
        Assert.Contains("page[size]=50", url);
    }

    [Fact]
    public void BuildApiUrl_WithDifferentDate_ShouldUseCorrectDateRange()
    {
        // Arrange
        var referenceDate = DateTimeOffset.Parse("2024-01-01");

        // Act
        var url = _service.BuildApiUrl("EUR", referenceDate);

        // Assert
        Assert.Contains("currency:eq:EUR", url);
        Assert.Contains("record_date:lte:2024-01-01", url);
    }

    [Fact]
    public void FindBestMatchingRate_WithValidData_ShouldReturnFirstValidRate()
    {
        // Arrange - API already filters and sorts by currency and date
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        var data = new List<TreasuryExchangeRateData>
        {
            new() { Country = "Austria", Currency = "Euro", ExchangeRate = "0.85", RecordDate = "2024-06-10" },
            new() { Country = "Belgium", Currency = "Euro", ExchangeRate = "0.87", RecordDate = "2024-05-15" }
        };

        // Act
        var result = _service.FindBestMatchingRate(data, "EUR", referenceDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("0.85", result.ExchangeRate);
        Assert.Equal("2024-06-10", result.RecordDate);
    }

    [Fact]
    public void FindBestMatchingRate_WithRateAfterReferenceDate_ShouldReturnNull()
    {
        // Arrange
        var referenceDate = DateTimeOffset.Parse("2024-06-10");
        var data = new List<TreasuryExchangeRateData>
        {
            new() { Country = "Austria", Currency = "Euro", ExchangeRate = "0.85", RecordDate = "2024-06-15" } // After reference date
        };

        // Act
        var result = _service.FindBestMatchingRate(data, "EUR", referenceDate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindBestMatchingRate_WithEmptyData_ShouldReturnNull()
    {
        // Arrange - API returns empty data when no currency matches
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        var data = new List<TreasuryExchangeRateData>();

        // Act
        var result = _service.FindBestMatchingRate(data, "EUR", referenceDate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindBestMatchingRate_WithValidDateConstraint_ShouldReturnMatch()
    {
        // Arrange - API filters by currency, we validate date constraint
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        var data = new List<TreasuryExchangeRateData>
        {
            new() { Country = "Austria", Currency = "Euro", ExchangeRate = "0.85", RecordDate = "2024-06-10" }
        };

        // Act
        var result = _service.FindBestMatchingRate(data, "EUR", referenceDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("0.85", result.ExchangeRate);
    }

    [Fact]
    public void CreateExchangeRateFromData_WithValidData_ShouldReturnExchangeRate()
    {
        // Arrange
        var data = new TreasuryExchangeRateData
        {
            Country = "Austria",
            Currency = "Euro",
            ExchangeRate = "0.85",
            RecordDate = "2024-06-10"
        };

        // Act
        var result = _service.CreateExchangeRateFromData(data, "EUR");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EUR", result.Currency);
        Assert.Equal(0.85m, result.Rate);
        Assert.Equal(DateTimeOffset.Parse("2024-06-10").Date, result.Date.Date);
    }

    [Fact]
    public void CreateExchangeRateFromData_WithInvalidExchangeRate_ShouldReturnNull()
    {
        // Arrange
        var data = new TreasuryExchangeRateData
        {
            Country = "Austria",
            Currency = "Euro",
            ExchangeRate = "invalid_rate",
            RecordDate = "2024-06-10"
        };

        // Act
        var result = _service.CreateExchangeRateFromData(data, "EUR");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CreateExchangeRateFromData_WithValidDecimalFormats_ShouldParseCorrectly()
    {
        // Test various valid decimal formats (must be positive)
        var testCases = new[]
        {
            ("0.85", 0.85m),
            ("1.0", 1.0m),
            ("1234.5678", 1234.5678m),
            ("0.01", 0.01m)
        };

        foreach (var (exchangeRateString, expectedRate) in testCases)
        {
            // Arrange
            var data = new TreasuryExchangeRateData
            {
                Country = "Test",
                Currency = "Currency",
                ExchangeRate = exchangeRateString,
                RecordDate = "2024-06-10"
            };

            // Act
            var result = _service.CreateExchangeRateFromData(data, "Test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRate, result.Rate);
        }
    }

}