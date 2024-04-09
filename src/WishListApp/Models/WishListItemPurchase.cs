using MongoDB.Bson;

namespace WishListApp.Models;

public sealed record WishListItemPurchase(
    ObjectId ShareId,
    int Quantity);
