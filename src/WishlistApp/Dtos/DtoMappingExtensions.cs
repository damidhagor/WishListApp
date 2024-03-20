namespace WishlistApp.Dtos;

public static class DtoMappingExtensions
{
    public static WishlistDto ToDto(this Data.Sql.Models.Wishlist wishlist)
        => new(
            wishlist.Id,
            wishlist.OwnerIdentifier,
            wishlist.Name,
            wishlist.Items.ToDtos().ToArray(),
            wishlist.Shares.ToDtos().ToArray());

    public static IEnumerable<WishlistDto> ToDtos(this IEnumerable<Data.Sql.Models.Wishlist> wishlists)
        => wishlists.Select(w => w.ToDto());

    public static WishlistItemDto ToDto(this Data.Sql.Models.WishlistItem item)
        => new(
            item.Id,
            item.WishlistId,
            item.Url,
            item.Name,
            item.Description,
            item.Note,
            item.Price,
            item.Quantity,
            item.Priority.ToDto(),
            item.Purchases.ToDtos().ToArray());

    public static IEnumerable<WishlistItemDto> ToDtos(this IEnumerable<Data.Sql.Models.WishlistItem> items)
        => items.Select(i => i.ToDto());

    public static WishlistShareDto ToDto(this Data.Sql.Models.WishlistShare share)
        => new(
            share.Id,
            share.WishlistId,
            share.Name,
            share.AccessKey,
            share.Purchases.ToDtos().ToArray(),
            share.IsDeleted);

    public static IEnumerable<WishlistShareDto> ToDtos(this IEnumerable<Data.Sql.Models.WishlistShare> shares)
        => shares.Select(s => s.ToDto());

    public static WishlistPurchaseDto ToDto(this Data.Sql.Models.WishlistPurchase buy)
        => new(
            buy.Id,
            buy.ItemId,
            buy.ShareId,
            buy.Quantity);

    public static IEnumerable<WishlistPurchaseDto> ToDtos(this IEnumerable<Data.Sql.Models.WishlistPurchase> buys)
        => buys.Select(b => b.ToDto());

    public static WishlistItemPriorityDto ToDto(this Data.Sql.Models.WishlistItemPriority priority)
        => new(priority, priority switch
        {
            Data.Sql.Models.WishlistItemPriority.Low => "Nicht unbedingt",
            Data.Sql.Models.WishlistItemPriority.Medium => "Hätte ich gerne",
            Data.Sql.Models.WishlistItemPriority.High => "Hätte ich sehr gerne",
            Data.Sql.Models.WishlistItemPriority.VeryHigh => "Muss ich haben",
            _ => "Unbekannt"
        });
}