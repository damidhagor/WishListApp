using System.ComponentModel.DataAnnotations;

namespace WishListApp.Data.Sql.Models;

public sealed class WishListItemEntity
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required WishListEntity Wishlist { get; init; }

    [MaxLength(500)]
    public required string Url { get; init; }

    [MaxLength(500)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Price { get; set; }

    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public WishListItemPriorityValue Priority { get; set; }

    public List<WishListPurchaseEntity> Purchases { get; set; } = [];
}
