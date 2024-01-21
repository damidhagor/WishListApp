namespace WishlistApp.Services.Repositories;

public interface IWishlistItemRepository
{
    Task<WishlistItemDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto updatedItem, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistItem(int itemId, CancellationToken cancellationToken);

    Task DeletePurchasedWishlistItems(int wishlistId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> SetWishlistItemPriority(int itemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken);
}
