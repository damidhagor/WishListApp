namespace WishListApp.Data.Sql.Models;

public sealed class WishListShareEntity
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required WishListEntity Wishlist { get; init; }

    public required string Name { get; init; }

    public required string AccessKey { get; init; }

    public List<WishListPurchaseEntity> Purchases { get; set; } = [];

    public bool IsDeleted { get; set; }
}
