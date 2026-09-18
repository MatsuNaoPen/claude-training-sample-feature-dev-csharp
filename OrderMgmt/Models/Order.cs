namespace OrderMgmt.Models;

/// <summary>注文。複数の明細を持つ。</summary>
public sealed class Order
{
    public int Id { get; init; }

    public int CustomerId { get; init; }

    public DateOnly OrderedOn { get; init; }

    public List<OrderLine> Lines { get; init; } = new();
}
