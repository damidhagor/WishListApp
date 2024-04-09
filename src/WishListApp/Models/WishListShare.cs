using MongoDB.Bson;

namespace WishListApp.Models;

public sealed record WishListShare(
    ObjectId Id,
    ObjectId WishListId,
    string Name,
    string AccessKey);
