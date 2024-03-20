namespace WishlistApp.Data.Models;

public sealed record WishlistShare(
    int Id,
    int WishlistId,
    string Name,
    string AccessKey,
    WishlistPurchase[] Purchases,
    bool IsDeleted);
