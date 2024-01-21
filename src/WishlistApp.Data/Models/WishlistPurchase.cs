namespace WishlistApp.Data.Models;

public sealed class WishlistPurchase
{
    public int Id { get; init; }

    public required int ItemId { get; init; }

    public required WishlistItem Item { get; init; }

    public required int ShareId { get; init; }

    public required WishlistShare Share { get; init; }

    public required int Quantity { get; set; }
}
