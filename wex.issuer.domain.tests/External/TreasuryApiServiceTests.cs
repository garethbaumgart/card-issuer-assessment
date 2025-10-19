using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using wex.issuer.domain.External;
using wex.issuer.domain.External.Models;

namespace wex.issuer.domain.tests.External;

public class TreasuryApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<TreasuryApiService>> _mockLogger;
    private readonly HttpClient _httpClient;
    private readonly TreasuryApiService _service;

    public TreasuryApiServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockLogger = new Mock<ILogger<TreasuryApiService>>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new TreasuryApiService(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithValidCurrency_ShouldReturnExchangeRate()
    {
        // Arrange
        var currency = "EUR";
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        var exchangeRateDate = DateTimeOffset.Parse("2024-06-10");
        
        var mockResponse = new TreasuryExchangeRateResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new()
                {
                    Country = "Austria",
                    Currency = "Euro",
                    ExchangeRate = "0.85",
                    RecordDate = "2024-06-10"
                }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetExchangeRateAsync(currency, referenceDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EUR", result.Currency);
        Assert.Equal(0.85m, result.Rate);
        Assert.Equal(exchangeRateDate.Date, result.Date.Date);
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithNoMatchingCurrency_ShouldReturnNull()
    {
        // Arrange - API filters by currency and returns empty data for non-existent currency
        var currency = "XYZ"; // Invalid 3-character currency code
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        
        var mockResponse = new TreasuryExchangeRateResponse
        {
            Data = new List<TreasuryExchangeRateData>() // Empty data for invalid currency
        };

        var jsonResponse = JsonSerializer.Serialize(mockResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetExchangeRateAsync(currency, referenceDate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithEmptyResponse_ShouldReturnNull()
    {
        // Arrange
        var currency = "EUR";
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        
        var mockResponse = new TreasuryExchangeRateResponse
        {
            Data = new List<TreasuryExchangeRateData>()
        };

        var jsonResponse = JsonSerializer.Serialize(mockResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetExchangeRateAsync(currency, referenceDate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithNullOrEmptyCurrency_ShouldThrowArgumentException()
    {
        // Arrange
        var referenceDate = DateTimeOffset.Parse("2024-06-15");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetExchangeRateAsync(null!, referenceDate));
        
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetExchangeRateAsync("", referenceDate));
        
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetExchangeRateAsync("   ", referenceDate));
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithHttpRequestException_ShouldThrowHttpRequestException()
    {
        // Arrange
        var currency = "EUR";
        var referenceDate = DateTimeOffset.Parse("2024-06-15");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _service.GetExchangeRateAsync(currency, referenceDate));
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithInvalidExchangeRateFormat_ShouldReturnNull()
    {
        // Arrange
        var currency = "EUR";
        var referenceDate = DateTimeOffset.Parse("2024-06-15");
        
        var mockResponse = new TreasuryExchangeRateResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new()
                {
                    Country = "Austria",
                    Currency = "Euro",
                    ExchangeRate = "invalid_rate",
                    RecordDate = "2024-06-10"
                }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetExchangeRateAsync(currency, referenceDate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_WithRateAfterReferenceDate_ShouldReturnNull()
    {
        // Arrange
        var currency = "EUR";
        var referenceDate = DateTimeOffset.Parse("2024-06-10");
        
        var mockResponse = new TreasuryExchangeRateResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new()
                {
                    Country = "Austria",
                    Currency = "Euro",
                    ExchangeRate = "0.85",
                    RecordDate = "2024-06-15" // After reference date
                }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetExchangeRateAsync(currency, referenceDate);

        // Assert
        Assert.Null(result);
    }

}