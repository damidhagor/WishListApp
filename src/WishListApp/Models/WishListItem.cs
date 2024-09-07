using MongoDB.Bson;

namespace WishListApp.Models;

public sealed record WishListItem(
    ObjectId Id,
    ObjectId WishListId,
    string Url,
    string? ImageUrl,
    string? SiteName,
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    string Note,
    int Priority,
    ObjectId? Purchaser)
{
    public static int[] AvailablePriorities { get; } = [0, 1, 2, 3, 4];

    public string NameOrUrl => string.IsNullOrWhiteSpace(Name) ? Url : Name;

    public bool IsPurchased => Purchaser is not null;

    public bool IsPurchasedByShare(ObjectId? shareId) => IsPurchased && Purchaser == shareId;

    public bool IsPurchasedByOtherShare(ObjectId? shareId) => IsPurchased && Purchaser != shareId;
}
