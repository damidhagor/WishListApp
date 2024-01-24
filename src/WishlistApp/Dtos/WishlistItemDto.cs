namespace WishlistApp.Dtos;

public sealed record WishlistItemDto(
    int Id,
    int WishlistId,
    string Url,
    string? Name,
    string? Description,
    string? Note,
    string? Price,
    int Quantity,
    WishlistItemPriorityDto Priority,
    WishlistPurchaseDto[] Purchases)
{
    public int PurchasedQuantity => Purchases.Sum(p => p.Quantity);

    public int RemainingQuantity => Math.Max(0, Quantity - PurchasedQuantity);

    public bool CanBePurchasedByShare(WishlistShareDto? share)
        => share is not null
        && Purchases.All(p => p.ShareId != share.Id)
        && RemainingQuantity > 0;

    public int GetPurchasedQuantityByShare(WishlistShareDto? share)
        => Purchases.Where(p => p.ShareId == share?.Id).Sum(b=>b.Quantity);
}
