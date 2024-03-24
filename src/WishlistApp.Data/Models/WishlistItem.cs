namespace WishListApp.Data.Models;

public sealed record WishListItem(
    string Url,
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    int Quantity,
    string Note,
    int Priority,
    WishListItemPurchase[] Purchases);
