namespace WishlistApp.Services.Repositories;

public interface IWishlistPurchaseRepository
{
    Task<WishlistPurchaseDto?> UpdatePurchaseQuantity(int itemId, int shareId, int quantity, CancellationToken cancellationToken);
}
