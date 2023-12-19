namespace WishlistApp.Dtos;

public sealed record WishlistDto(
    int Id,
    string OwnerIdentifier,
    string Name,
    WishlistItemDto[] Items,
    WishlistShareDto[] Shares);