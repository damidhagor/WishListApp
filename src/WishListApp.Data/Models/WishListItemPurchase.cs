namespace WishListApp.Data.Models;

public sealed record WishListItemPurchase(
    int Quantity,
    ObjectId ShareId,
    string ShareName);
