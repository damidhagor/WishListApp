using WishlistApp.Data.Models;

namespace WishlistApp.Data.Repositories;

public interface IWishlistRepository
{
    Task<Wishlist> CreateWishlist(string name, string ownerIdentifier, CancellationToken cancellationToken);

    Task<Wishlist?> RenameWishlist(int wishlistId, string name, CancellationToken cancellationToken);

    Task<Wishlist?> GetWishlist(int wishlistId, CancellationToken cancellationToken);

    Task<List<Wishlist>> GetAll(string ownerIdentifier, CancellationToken cancellationToken);

    Task<bool> DeleteWishlist(int wishlistId, CancellationToken cancellationToken);
}
