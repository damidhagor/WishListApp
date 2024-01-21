using System.ComponentModel.DataAnnotations;

namespace WishlistApp.Data.Models;

public sealed class WishlistItem
{
    public int Id { get; init; } = 0;

    public required int WishlistId { get; init; }

    public required Wishlist Wishlist { get; init; }

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

    public WishlistItemPriority Priority { get; set; }

    public List<WishlistPurchase> Purchases { get; set; } = [];
}
