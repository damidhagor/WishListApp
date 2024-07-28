using MongoDB.Bson;

namespace WishListApp.Models;

public sealed record WishListItem(
    ObjectId Id,
    ObjectId WishListId,
    string Url,
    string? ImageUrl,
    string? SiteName,
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    int Quantity,
    string Note,
    int Priority,
    WishListItemPurchase[] Purchases)
{
    public static int[] AvailablePriorities { get; } = [0, 1, 2, 3, 4];

    public string NameOrUrl => string.IsNullOrWhiteSpace(Name) ? Url : Name;

    public int PurchasedQuantity { get; } = Purchases.Sum(p => p.Quantity);

    public int RemainingQuantity { get; } = Math.Max(0, Quantity - Purchases.Sum(p => p.Quantity));

    public int GetPurchasedQuantityByShare(ObjectId? shareId) => Purchases.FirstOrDefault(p => p.ShareId == shareId)?.Quantity ?? 0;

    public bool CanBePurchasedByShare(ObjectId? shareId)
        => shareId is not null
        && Purchases.All(p => p.ShareId != shareId)
        && RemainingQuantity > 0;
}
