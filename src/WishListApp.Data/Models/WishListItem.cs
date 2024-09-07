namespace WishListApp.Data.Models;

public sealed record WishListItem(
    ObjectId Id,
    string Url,
    string? ImageUrl,
    string? SiteName,
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    int Quantity,
    string Note,
    int Priority,
    ObjectId? Purchaser);
