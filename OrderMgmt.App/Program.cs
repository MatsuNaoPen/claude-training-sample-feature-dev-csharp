using OrderMgmt.Infrastructure;
using OrderMgmt.Models;
using OrderMgmt.Services;

var exchangeRateService = new ExchangeRateService(new HttpClientWrapper("https://rates.example.invalid"));
var totalService = new OrderTotalService(exchangeRateService);

var orders = new List<Order>
{
    new()
    {
        Id = 1001, CustomerId = 1, OrderedOn = new DateOnly(2026, 1, 15),
        Lines = new List<OrderLine>
        {
            new() { Id = 1, ProductId = 101, Quantity = 2, UnitPrice = 1200, ShippingFee = 300 },
            new() { Id = 2, ProductId = 102, Quantity = 0, UnitPrice = 500, ShippingFee = 300 },
            new() { Id = 3, ProductId = 103, Quantity = 1, UnitPrice = 5000, ShippingFee = 0 },
        },
    },
    new()
    {
        Id = 1002, CustomerId = 2, OrderedOn = new DateOnly(2026, 1, 16),
        Lines = new List<OrderLine>
        {
            new() { Id = 4, ProductId = 104, Quantity = 10, UnitPrice = 800, ShippingFee = 500 },
            new() { Id = 5, ProductId = 105, Quantity = 3, UnitPrice = 1500, ShippingFee = 0 },
        },
    },
    new()
    {
        Id = 1003, CustomerId = 1, OrderedOn = new DateOnly(2026, 1, 20),
        Lines = new List<OrderLine>
        {
            new() { Id = 6, ProductId = 106, Quantity = 0, UnitPrice = 2000, ShippingFee = 500 },
        },
    },
};

foreach (var order in orders)
{
    Console.WriteLine($"注文 {order.Id}（{order.OrderedOn:yyyy-MM-dd}）");
    foreach (var line in order.Lines)
    {
        Console.WriteLine(
            $"  明細 {line.Id}: 数量 {line.Quantity,3} × 単価 {line.UnitPrice,6:N0} + 送料 {line.ShippingFee,4:N0} = {totalService.CalculateLineAmount(line),8:N0} 円");
    }

    Console.WriteLine($"  注文合計: {totalService.CalculateOrderTotal(order):N0} 円");
}

var grandTotalJpy = await totalService.CalculateGrandTotalAsync(orders);
var grandTotalUsd = await totalService.CalculateGrandTotalAsync(orders, "USD");
Console.WriteLine($"一覧合計: {grandTotalJpy:N0} 円（{grandTotalUsd:N2} USD）");
