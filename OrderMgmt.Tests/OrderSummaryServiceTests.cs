using OrderMgmt.Models;
using OrderMgmt.Services;

namespace OrderMgmt.Tests;

public class OrderSummaryServiceTests
{
    private readonly OrderSummaryService _sut = new();

    [Fact]
    public void CountLines_全注文の明細数を数える()
    {
        var orders = new[]
        {
            TestData.Order(1, TestData.Line(1, 1, 100), TestData.Line(2, 1, 100)),
            TestData.Order(2, TestData.Line(3, 1, 100)),
        };

        Assert.Equal(3, _sut.CountLines(orders));
    }

    [Fact]
    public void CountOrdersByRank_顧客ランクごとに件数を集計する()
    {
        var customers = new Dictionary<int, Customer>
        {
            [1] = new() { Id = 1, Name = "取引先A", Rank = CustomerRank.A },
        };
        var orders = new[] { TestData.Order(1), TestData.Order(2) };

        var result = _sut.CountOrdersByRank(orders, customers);

        Assert.Equal(2, result[CustomerRank.A]);
    }
}
