using System.Text.Json;
using OrderMgmt.Infrastructure;

namespace OrderMgmt.Services;

/// <summary>為替レートを社内レート API から取得する。</summary>
public sealed class ExchangeRateService
{
    private readonly HttpClientWrapper _httpClient;

    public ExchangeRateService(HttpClientWrapper httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>円から指定通貨への換算レートを返す。</summary>
    public async Task<decimal> GetRateAsync(string currency, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("通貨コードは必須です。", nameof(currency));
        }

        var json = await _httpClient.GetStringAsync($"/rates/{currency.ToUpperInvariant()}", cancellationToken);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("rate").GetDecimal();
    }
}
