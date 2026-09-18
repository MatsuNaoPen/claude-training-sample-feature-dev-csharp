using OrderMgmt.Services;

namespace OrderMgmt.Tests;

public class OrderTotalServiceTests
{
    private readonly OrderTotalService _sut = TestData.CreateTotalService();

    [Fact]
    public void CalculateLineAmount_数量と単価を掛けて送料を足す()
    {
        var line = TestData.Line(1, quantity: 3, unitPrice: 1200, shippingFee: 500);

        Assert.Equal(4100m, _sut.CalculateLineAmount(line));
    }


    [Fact]
    public void CalculateLineAmount_数量が上限を超えると例外()
    {
        var line = TestData.Line(1, quantity: OrderTotalService.MaxQuantity + 1, unitPrice: 100);

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CalculateLineAmount(line));
    }

    [Fact]
    public void CalculateLineAmount_数量が上限ちょうどなら計算できる()
    {
        var line = TestData.Line(1, quantity: OrderTotalService.MaxQuantity, unitPrice: 1);

        Assert.Equal(9999m, _sut.CalculateLineAmount(line));
    }

    [Fact]
    public void CalculateOrderTotal_明細金額を合計する()
    {
        var order = TestData.Order(1,
            TestData.Line(1, quantity: 2, unitPrice: 1000, shippingFee: 300),
            TestData.Line(2, quantity: 1, unitPrice: 5000, shippingFee: 300));

        Assert.Equal(7600m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void CalculateOrderTotal_明細がなければ0()
    {
        var order = TestData.Order(1);

        Assert.Equal(0m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void CalculateOrderTotal_注文がnullなら例外()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.CalculateOrderTotal(null!));
    }

    [Fact]
    public async Task CalculateGrandTotalAsync_複数注文を合計する()
    {
        var orders = new[]
        {
            TestData.Order(1, TestData.Line(1, quantity: 1, unitPrice: 1000)),
            TestData.Order(2, TestData.Line(2, quantity: 2, unitPrice: 2500, shippingFee: 200)),
        };

        Assert.Equal(6200m, await _sut.CalculateGrandTotalAsync(orders));
    }

    [Fact]
    public async Task CalculateGrandTotalAsync_通貨を指定すると換算する()
    {
        var orders = new[] { TestData.Order(1, TestData.Line(1, quantity: 1, unitPrice: 10000)) };

        Assert.Equal(68.00m, await _sut.CalculateGrandTotalAsync(orders, "USD"));
    }
}
