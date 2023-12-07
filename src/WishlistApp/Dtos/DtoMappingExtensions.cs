namespace WishlistApp.Dtos;

public static class DtoMappingExtensions
{
    public static WishlistDto ToDto(this Data.Models.Wishlist wishlist)
        => new()
        {
            Id = wishlist.Id,
            Name = wishlist.Name,
            Items = wishlist.Items.ToDtos().ToArray(),
            Shares = wishlist.Shares.ToDtos().ToArray()
        };

    public static IEnumerable<WishlistDto> ToDtos(this IEnumerable<Data.Models.Wishlist> wishlists)
        => wishlists.Select(w => w.ToDto());

    public static WishlistItemDto ToDto(this Data.Models.WishlistItem item)
        => new()
        {
            Id = item.Id,
            WishlistId = item.WishlistId,
            Url = item.Url,
            Order = item.Order,
            BoughtByWishlistShareId = item.BoughtByWishlistShareId
        };

    public static IEnumerable<WishlistItemDto> ToDtos(this IEnumerable<Data.Models.WishlistItem> items)
        => items.Select(i => i.ToDto());

    public static WishlistShareDto ToDto(this Data.Models.WishlistShare share)
        => new()
        {
            Id = share.Id,
            WishlistId = share.WishlistId,
            Name = share.Name,
            AccessKey = share.AccessKey,
            IsDeleted = share.IsDeleted
        };

    public static IEnumerable<WishlistShareDto> ToDtos(this IEnumerable<Data.Models.WishlistShare> shares)
        => shares.Select(s => s.ToDto());
}