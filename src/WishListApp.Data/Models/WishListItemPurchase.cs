namespace WishListApp.Data.Models;

public sealed record WishListItemPurchase(
    ObjectId ShareId,
    int Quantity);
