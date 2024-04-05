namespace WishListApp.Data.Models;

public sealed record WishListShare(
    ObjectId Id,
    ObjectId WishListId,
    string Name,
    string AccessKey);
