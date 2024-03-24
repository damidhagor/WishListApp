namespace WishlistApp.Data.Sql.Models;

public sealed class WishlistShareEntity
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required WishlistEntity Wishlist { get; init; }

    public required string Name { get; init; }

    public required string AccessKey { get; init; }

    public List<WishlistPurchaseEntity> Purchases { get; set; } = [];

    public bool IsDeleted { get; set; }
}
