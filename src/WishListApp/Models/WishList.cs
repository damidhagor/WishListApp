using MongoDB.Bson;

namespace WishListApp.Models;

public sealed record WishList(
    ObjectId Id,
    string OwnerId,
    string Name,
    WishListItem[] Items);
