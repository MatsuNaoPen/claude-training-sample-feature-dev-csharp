using OrderMgmt.Infrastructure;
using OrderMgmt.Services;

namespace OrderMgmt.Tests;

public class ExchangeRateServiceTests
{
    private readonly ExchangeRateService _sut = new(new HttpClientWrapper("https://rates.example.invalid"));

    [Fact]
    public async Task GetRateAsync_円は1を返す()
    {
        Assert.Equal(1.0m, await _sut.GetRateAsync("JPY"));
    }


    [Fact]
    public async Task GetRateAsync_通貨コードが空なら例外()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetRateAsync(""));
    }
}
