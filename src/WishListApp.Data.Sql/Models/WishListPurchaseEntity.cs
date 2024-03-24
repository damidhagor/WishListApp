namespace WishListApp.Data.Sql.Models;

public sealed class WishListPurchaseEntity
{
    public int Id { get; init; }

    public required int ItemId { get; init; }

    public required WishListItemEntity Item { get; init; }

    public required int ShareId { get; init; }

    public required WishListShareEntity Share { get; init; }

    public required int Quantity { get; set; }
}
