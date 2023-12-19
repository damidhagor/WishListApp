namespace WishlistApp.Dtos;

public sealed record WishlistItemDto(
    int Id,
    int WishlistId,
    string Url,
    string? Name,
    string? Description,
    string? Note,
    string? Price,
    WishlistItemPriorityDto Priority,
    int? BoughtByWishlistShareId);
