using WishlistApp.Data.Models;

namespace WishlistApp.Data.Sql.Models;

public static class MappingExtensions
{
    public static Wishlist ToModel(this WishlistEntity wishlist)
        => new(
            wishlist.Id,
            wishlist.OwnerIdentifier,
            wishlist.Name,
            wishlist.Items.ToModels().ToArray(),
            wishlist.Shares.ToModels().ToArray());

    public static IEnumerable<Wishlist> ToModels(this IEnumerable<WishlistEntity> wishlists)
        => wishlists.Select(w => w.ToModel());

    public static WishlistItem ToModel(this WishlistItemEntity item)
        => new(
            item.Id,
            item.WishlistId,
            item.Url,
            item.Name,
            item.Description,
            item.Note,
            item.Price,
            item.Quantity,
            item.Priority.ToModel(),
            item.Purchases.ToModels().ToArray());

    public static IEnumerable<WishlistItem> ToModels(this IEnumerable<WishlistItemEntity> items)
        => items.Select(i => i.ToModel());

    public static WishlistShare ToModel(this WishlistShareEntity share)
        => new(
            share.Id,
            share.WishlistId,
            share.Name,
            share.AccessKey,
            share.Purchases.ToModels().ToArray(),
            share.IsDeleted);

    public static IEnumerable<WishlistShare> ToModels(this IEnumerable<WishlistShareEntity> shares)
        => shares.Select(s => s.ToModel());

    public static WishlistPurchase ToModel(this WishlistPurchaseEntity buy)
        => new(
            buy.Id,
            buy.ItemId,
            buy.ShareId,
            buy.Quantity);

    public static IEnumerable<WishlistPurchase> ToModels(this IEnumerable<WishlistPurchaseEntity> buys)
        => buys.Select(b => b.ToModel());
}