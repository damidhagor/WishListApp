namespace WishlistApp.Data.Sql.Models;

public sealed class WishlistPurchaseEntity
{
    public int Id { get; init; }

    public required int ItemId { get; init; }

    public required WishlistItemEntity Item { get; init; }

    public required int ShareId { get; init; }

    public required WishlistShareEntity Share { get; init; }

    public required int Quantity { get; set; }
}
