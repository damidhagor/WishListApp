namespace WishlistApp.Data.Models;

public sealed class WishlistShare
{
    public required int Id { get; init; }

    public required int WishlistId { get; init; }

    public required Wishlist Wishlist { get; init; }

    public required string Name { get; init; }

    public required string AccessKey { get; init; }

    public List<WishlistItem> BoughtItems { get; set; } = [];
}
