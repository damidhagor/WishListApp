namespace WishlistApp.Dtos;

public sealed record WishlistPurchaseDto(
    int Id,
    int ItemId,
    int ShareId,
    int Quantity);
