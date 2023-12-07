namespace WishlistApp.Dtos;

public sealed class WishlistItemDto
{
    public required int Id { get; init; }

    public required int WishlistId { get; init; }

    public required string Url { get; init; }

    public required WishlistItemPriorityDto Priority { get; init; }

    public int? BoughtByWishlistShareId { get; set; }
}
