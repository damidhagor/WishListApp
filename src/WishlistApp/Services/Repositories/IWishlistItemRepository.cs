namespace WishlistApp.Services.Repositories;

public interface IWishlistItemRepository
{
    Task<WishlistItemDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto wishlistItemDto, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistItem(int wishlistItemId, CancellationToken cancellationToken);

    Task DeleteBoughtWishlistItems(int wishlistId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> BuyWishlistItem(int wishlistItemId, int wishlistShareId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> UnbuyWishlistItem(int wishlistItemId, CancellationToken cancellationToken);

    Task<WishlistItemDto?> SetWishlistItemPriority(int wishlistItemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken);
}
