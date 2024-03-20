namespace WishlistApp.Data.Models;

public sealed record WishlistPurchase(
    int Id,
    int ItemId,
    int ShareId,
    int Quantity);
