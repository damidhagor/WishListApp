namespace WishListApp.Data.Models;

public sealed record WishListItem(
    ObjectId Id,
    string Url,
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    int Quantity,
    string Note,
    int Priority,
    WishListItemPurchase[] Purchases);

// Unknown = 0,
// Low = 1,
// Medium = 2,
// High = 3,
// VeryHigh = 4
