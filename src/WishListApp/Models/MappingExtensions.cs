using MongoDB.Bson;

namespace WishListApp.Models;

internal static class MappingExtensions
{
    public static WishList ToModel(this Data.Models.WishList wishList)
        => new(
            wishList.Id,
            wishList.OwnerId,
            wishList.Name,
            wishList.Items.ToModels(wishList.Id).ToArray());

    public static IEnumerable<WishList> ToModels(this IEnumerable<Data.Models.WishList> wishLists)
        => wishLists.Select(w => w.ToModel());

    public static WishListItem ToModel(this Data.Models.WishListItem item, ObjectId wishListId)
        => new(
            item.Id,
            wishListId,
            item.Url,
            item.ImageUrl,
            item.SiteName,
            item.Name,
            item.Description,
            item.Price,
            item.Currency,
            item.Quantity,
            item.Note,
            item.Priority.ToModel(),
            item.Purchases.ToModels().ToArray());

    public static IEnumerable<WishListItem> ToModels(this IEnumerable<Data.Models.WishListItem> items, ObjectId wishListId)
        => items.Select(i => i.ToModel(wishListId));

    public static Data.Models.WishListItem ToDataModel(this WishListItem item)
        => new(
            item.Id,
            item.Url,
            item.ImageUrl,
            item.SiteName,
            item.Name,
            item.Description,
            item.Price,
            item.Currency,
            item.Quantity,
            item.Note,
            item.Priority.Priority,
            item.Purchases.ToDataModels().ToArray());

    public static IEnumerable<Data.Models.WishListItem> ToDataModels(this IEnumerable<WishListItem> items)
        => items.Select(i => i.ToDataModel());

    public static WishListItemPurchase ToModel(this Data.Models.WishListItemPurchase purchase)
        => new(purchase.ShareId, purchase.Quantity);

    public static IEnumerable<WishListItemPurchase> ToModels(this IEnumerable<Data.Models.WishListItemPurchase> purchases)
        => purchases.Select(p => p.ToModel());

    public static Data.Models.WishListItemPurchase ToDataModel(this WishListItemPurchase purchase)
        => new(purchase.ShareId, purchase.Quantity);

    public static IEnumerable<Data.Models.WishListItemPurchase> ToDataModels(this IEnumerable<WishListItemPurchase> purchases)
        => purchases.Select(p => p.ToDataModel());

    public static WishListShare ToModel(this Data.Models.WishListShare share)
        => new(share.Id, share.WishListId, share.Name, share.AccessKey);

    public static IEnumerable<WishListShare> ToModels(this IEnumerable<Data.Models.WishListShare> shares)
        => shares.Select(s => s.ToModel());

    public static WishListItemPriority ToModel(this int priority)
        => priority switch
        {
            0 => new WishListItemPriority(0, "Unbekannt"),
            1 => new WishListItemPriority(1, "Nicht unbedingt"),
            2 => new WishListItemPriority(2, "Hätte ich gerne"),
            3 => new WishListItemPriority(3, "Hätte ich sehr gerne"),
            4 => new WishListItemPriority(4, "Muss ich haben"),
            _ => new WishListItemPriority(priority, priority.ToString())
        };
}
