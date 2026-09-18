using OrderMgmt.Models;

namespace OrderMgmt.Services;

/// <summary>注文一覧の集計（件数・明細数・顧客ランク別の内訳）。表示用。</summary>
public sealed class OrderSummaryService
{
    public int CountOrders(IReadOnlyList<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        return orders.Count;
    }

    public int CountLines(IReadOnlyList<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var count = 0;
        foreach (var order in orders)
        {
            count += order.Lines.Count;
        }

        return count;
    }

    public Dictionary<CustomerRank, int> CountOrdersByRank(
        IReadOnlyList<Order> orders,
        IReadOnlyDictionary<int, Customer> customersById)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(customersById);

        var result = new Dictionary<CustomerRank, int>();
        foreach (var order in orders)
        {
            var rank = customersById.TryGetValue(order.CustomerId, out var customer)
                ? customer.Rank
                : CustomerRank.C;
            result[rank] = result.TryGetValue(rank, out var current) ? current + 1 : 1;
        }

        return result;
    }
}
