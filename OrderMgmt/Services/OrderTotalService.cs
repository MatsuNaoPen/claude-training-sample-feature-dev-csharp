using OrderMgmt.Models;

namespace OrderMgmt.Services;

/// <summary>注文金額の計算。明細金額・注文合計・一覧全体の合計を扱う。</summary>
public sealed class OrderTotalService
{
    /// <summary>1明細あたりの数量上限。</summary>
    public const int MaxQuantity = 9999;

    private readonly ExchangeRateService _exchangeRateService;

    public OrderTotalService(ExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    /// <summary>明細金額 = 数量 × 単価 + 明細送料。</summary>
    public decimal CalculateLineAmount(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (line.Quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(line), "数量は 0 以上にしてください。");
        }

        if (line.Quantity > MaxQuantity)
        {
            throw new ArgumentOutOfRangeException(nameof(line), $"数量は {MaxQuantity} 以下にしてください。");
        }

        return line.Quantity * line.UnitPrice + line.ShippingFee;
    }

    /// <summary>注文合計 = 明細金額の合計。数量 0 の明細は合計に含めない（docs/spec-exclude-zero-qty.md）。</summary>
    public decimal CalculateOrderTotal(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        decimal total = 0;
        foreach (var line in order.Lines)
        {
            ArgumentNullException.ThrowIfNull(line);

            if (line.Quantity == 0)
            {
                continue;
            }

            total += CalculateLineAmount(line);
        }

        return total;
    }

    /// <summary>一覧全体の合計を指定通貨に換算して返す（小数第2位で四捨五入）。</summary>
    public async Task<decimal> CalculateGrandTotalAsync(
        IReadOnlyList<Order> orders,
        string currency = "JPY",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orders);

        decimal totalInYen = 0;
        foreach (var order in orders)
        {
            totalInYen += CalculateOrderTotal(order);
        }

        var rate = await _exchangeRateService.GetRateAsync(currency, cancellationToken);
        return Math.Round(totalInYen * rate, 2, MidpointRounding.AwayFromZero);
    }
}
