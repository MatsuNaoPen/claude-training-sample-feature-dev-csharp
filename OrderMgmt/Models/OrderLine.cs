namespace OrderMgmt.Models;

/// <summary>注文明細。1商品につき1行。</summary>
public sealed class OrderLine
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    /// <summary>数量。</summary>
    public int Quantity { get; init; }

    /// <summary>単価（円）。</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>明細ごとの送料（円）。商品の梱包区分によって決まる。</summary>
    public decimal ShippingFee { get; init; }
}
