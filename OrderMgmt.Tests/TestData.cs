using OrderMgmt.Infrastructure;
using OrderMgmt.Models;
using OrderMgmt.Services;

namespace OrderMgmt.Tests;

internal static class TestData
{
    public static OrderTotalService CreateTotalService()
        => new(new ExchangeRateService(new HttpClientWrapper("https://rates.example.invalid")));

    public static OrderLine Line(int id, int quantity, decimal unitPrice, decimal shippingFee = 0)
        => new() { Id = id, ProductId = 100 + id, Quantity = quantity, UnitPrice = unitPrice, ShippingFee = shippingFee };

    public static Order Order(int id, params OrderLine[] lines)
        => new() { Id = id, CustomerId = 1, OrderedOn = new DateOnly(2026, 1, 15), Lines = lines.ToList() };
}
