namespace WishlistApp.Data.Models;

public sealed record WishlistItem(
    int Id,
    int WishlistId,
    string Url,
    string? Name,
    string? Description,
    string? Note,
    string? Price,
    int Quantity,
    WishlistItemPriority Priority,
    WishlistPurchase[] Purchases)
{
    public int PurchasedQuantity => Purchases.Sum(p => p.Quantity);

    public int RemainingQuantity => Math.Max(0, Quantity - PurchasedQuantity);

    public bool CanBePurchasedByShare(WishlistShare? share)
        => share is not null
        && Purchases.All(p => p.ShareId != share.Id)
        && RemainingQuantity > 0;

    public int GetPurchasedQuantityByShare(WishlistShare? share)
        => Purchases.Where(p => p.ShareId == share?.Id).Sum(b => b.Quantity);
}
