namespace WishlistApp.Data.Models;

public sealed class WishlistItem
{
    public required int Id { get; init; }

    public required int WishlistId { get; init; }

    public required Wishlist Wishlist { get; init; }

    public required string Url { get; init; }

    public int? BoughtByWishlistShareId { get; set; }

    public WishlistShare? BoughtByWishlistShare { get; set; }

    public bool IsBought => BoughtByWishlistShareId is not null;
}
