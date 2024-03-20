using WishlistApp.Data.Models;

namespace WishlistApp.Data.Repositories;

public interface IWishlistItemRepository
{
    Task<WishlistItem?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<WishlistItem?> UpdateWishlistItem(WishlistItem updatedItem, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistItem(int itemId, CancellationToken cancellationToken);

    Task DeletePurchasedWishlistItems(int wishlistId, CancellationToken cancellationToken);

    Task<WishlistItem?> SetWishlistItemPriority(int itemId, WishlistItemPriority priority, CancellationToken cancellationToken);
}
