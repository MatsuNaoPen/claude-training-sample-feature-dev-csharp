namespace OrderMgmt.Models;

/// <summary>取引先。</summary>
public sealed class Customer
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public CustomerRank Rank { get; init; } = CustomerRank.C;
}
