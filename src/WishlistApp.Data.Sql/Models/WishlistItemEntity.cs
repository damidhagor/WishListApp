using System.ComponentModel.DataAnnotations;
using WishlistApp.Data.Models;

namespace WishlistApp.Data.Sql.Models;

public sealed class WishlistItemEntity
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required WishlistEntity Wishlist { get; init; }

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

    public WishlistItemPriorityValue Priority { get; set; }

    public List<WishlistPurchaseEntity> Purchases { get; set; } = [];
}
