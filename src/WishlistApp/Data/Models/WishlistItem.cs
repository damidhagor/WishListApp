namespace WishlistApp.Data.Models;

public sealed class WishlistItem
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required Wishlist Wishlist { get; init; }

    public required string Url { get; init; }

    public WishlistItemPriority Priority { get; init; }

    public int? BoughtByWishlistShareId { get; set; }

    public WishlistShare? BoughtByWishlistShare { get; set; }

    public bool IsBought => BoughtByWishlistShareId is not null;
}
