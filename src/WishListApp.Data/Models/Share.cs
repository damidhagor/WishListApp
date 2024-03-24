namespace WishListApp.Data.Models;

public sealed record Share(
    ObjectId Id,
    ObjectId WishListId,
    string Name,
    string AccessKey);
