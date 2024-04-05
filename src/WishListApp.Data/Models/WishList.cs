namespace WishListApp.Data.Models;

public sealed record WishList(
    ObjectId Id,
    string OwnerId,
    string Name,
    WishListItem[] Items);
