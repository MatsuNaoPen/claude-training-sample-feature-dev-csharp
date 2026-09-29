using OrderMgmt.Models;
using OrderMgmt.Services;

namespace OrderMgmt.Tests;

/// <summary>仕様: docs/spec-exclude-zero-qty.md ／ 観点: docs/test-matrix-exclude-zero-qty.md</summary>
public class ExcludeZeroQuantityTests
{
    private readonly OrderTotalService _sut = TestData.CreateTotalService();

    [Fact]
    public void 正常系_数量1以上の明細は合計に含める()
    {
        var order = TestData.Order(1, TestData.Line(1, quantity: 2, unitPrice: 1000, shippingFee: 300));

        Assert.Equal(2300m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 正常系_数量0の明細は送料ごと合計から除外する()
    {
        var order = TestData.Order(1,
            TestData.Line(1, quantity: 0, unitPrice: 500, shippingFee: 300),
            TestData.Line(2, quantity: 1, unitPrice: 1000));

        Assert.Equal(1000m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 正常系_数量0と1以上が混在しても数量0以外だけ合計する()
    {
        var order = TestData.Order(1,
            TestData.Line(1, quantity: 1, unitPrice: 100),
            TestData.Line(2, quantity: 0, unitPrice: 999),
            TestData.Line(3, quantity: 3, unitPrice: 200));

        Assert.Equal(700m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public async Task 正常系_一覧全体の合計でも数量0の明細は除外する()
    {
        var orders = new[]
        {
            TestData.Order(1, TestData.Line(1, quantity: 0, unitPrice: 5000)),
            TestData.Order(2, TestData.Line(2, quantity: 2, unitPrice: 1500)),
        };

        Assert.Equal(3000m, await _sut.CalculateGrandTotalAsync(orders));
    }

    [Fact]
    public void 境界値_数量1は含める()
    {
        var order = TestData.Order(1, TestData.Line(1, quantity: 1, unitPrice: 800));

        Assert.Equal(800m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 境界値_数量0の明細だけなら合計は0()
    {
        var order = TestData.Order(1, TestData.Line(1, quantity: 0, unitPrice: 800));

        Assert.Equal(0m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 境界値_負数の数量は例外()
    {
        var order = TestData.Order(1, TestData.Line(1, quantity: -1, unitPrice: 800));

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 境界値_上限9999は含める()
    {
        var order = TestData.Order(1, TestData.Line(1, quantity: OrderTotalService.MaxQuantity, unitPrice: 1));

        Assert.Equal(9999m, _sut.CalculateOrderTotal(order));
    }

    [Fact]
    public void 異常系_明細にnullが混ざっていれば例外()
    {
        var order = new Order { Id = 1, CustomerId = 1, Lines = new List<OrderLine> { null! } };

        Assert.Throws<ArgumentNullException>(() => _sut.CalculateOrderTotal(order));
    }
}
