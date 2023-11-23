namespace WishlistApp.Services;

public interface IWishlistRepository
{
    Task<Wishlist> CreateWishlist(string name, CancellationToken cancellationToken);

    Task<Wishlist?> RenameWishlist(int id, string name, CancellationToken cancellationToken);

    Task<Wishlist?> GetWishlist(int id, CancellationToken cancellationToken);

    Task<List<Wishlist>> GetAll(CancellationToken cancellationToken);

    Task DeleteWishlist(int id, CancellationToken cancellationToken);

    Task<Wishlist?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<Wishlist?> DeleteWishlistItem(int wishlistId, int wishlistItemId, CancellationToken cancellationToken);

    Task<Wishlist?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken);

    Task<Wishlist?> DeleteWishlistShare(int wishlistId, int wishlistShareId, CancellationToken cancellationToken);
}
