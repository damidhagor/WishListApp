namespace WishlistApp.Services.Repositories;

public interface IWishlistItemRepository
{
    Task<WishlistItemDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto updatedItem, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistItem(int itemId, CancellationToken cancellationToken);

    Task DeleteBoughtWishlistItems(int wishlistId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> BuyWishlistItem(int itemId, int shareId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> UnbuyWishlistItem(int itemId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> SetWishlistItemPriority(int itemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken);
}
