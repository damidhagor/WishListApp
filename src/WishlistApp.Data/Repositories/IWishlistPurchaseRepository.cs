using WishlistApp.Data.Models;

namespace WishlistApp.Data.Repositories;

public interface IWishlistPurchaseRepository
{
    Task<WishlistPurchase?> UpdatePurchaseQuantity(int itemId, int shareId, int quantity, CancellationToken cancellationToken);
}
