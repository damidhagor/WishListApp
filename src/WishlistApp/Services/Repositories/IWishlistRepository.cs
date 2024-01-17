namespace WishlistApp.Services.Repositories;

public interface IWishlistRepository
{
    Task<WishlistDto> CreateWishlist(string name, string ownerIdentifier, CancellationToken cancellationToken);

    Task<WishlistDto?> RenameWishlist(int wishlistId, string name, CancellationToken cancellationToken);

    Task<WishlistDto?> GetWishlist(int wishlistId, CancellationToken cancellationToken);

    Task<List<WishlistDto>> GetAll(string ownerIdentifier, CancellationToken cancellationToken);

    Task<bool> DeleteWishlist(int wishlistId, CancellationToken cancellationToken);
}
