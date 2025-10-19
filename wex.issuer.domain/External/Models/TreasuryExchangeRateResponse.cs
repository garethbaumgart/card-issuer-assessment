using System.Text.Json.Serialization;

namespace wex.issuer.domain.External.Models;

public record TreasuryExchangeRateResponse
{
    [JsonPropertyName("data")]
    public IList<TreasuryExchangeRateData> Data { get; init; } = new List<TreasuryExchangeRateData>();

    [JsonPropertyName("meta")]
    public TreasuryMeta Meta { get; init; } = new();

    [JsonPropertyName("links")]
    public TreasuryLinks Links { get; init; } = new();
}

public record TreasuryExchangeRateData
{
    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("exchange_rate")]
    public string ExchangeRate { get; init; } = string.Empty;

    [JsonPropertyName("record_date")]
    public string RecordDate { get; init; } = string.Empty;
}

public record TreasuryMeta
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("total-count")]
    public int TotalCount { get; init; }

    [JsonPropertyName("total-pages")]
    public int TotalPages { get; init; }
}

public record TreasuryLinks
{
    [JsonPropertyName("self")]
    public string Self { get; init; } = string.Empty;

    [JsonPropertyName("first")]
    public string First { get; init; } = string.Empty;

    [JsonPropertyName("prev")]
    public string? Prev { get; init; }

    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonPropertyName("last")]
    public string Last { get; init; } = string.Empty;
}