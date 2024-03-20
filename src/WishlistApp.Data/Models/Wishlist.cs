namespace WishlistApp.Data.Models;

public sealed record Wishlist(
    int Id,
    string OwnerIdentifier,
    string Name,
    WishlistItem[] Items,
    WishlistShare[] Shares);