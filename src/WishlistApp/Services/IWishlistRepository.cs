namespace WishlistApp.Services;

public interface IWishlistRepository
{
    Task<WishlistDto> CreateWishlist(string name, CancellationToken cancellationToken);

    Task<WishlistDto?> RenameWishlist(int id, string name, CancellationToken cancellationToken);

    Task<WishlistDto?> GetWishlist(int id, CancellationToken cancellationToken);

    Task<List<WishlistDto>> GetAll(CancellationToken cancellationToken);

    Task DeleteWishlist(int id, CancellationToken cancellationToken);

    Task<WishlistDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken);

    Task<WishlistDto?> DeleteWishlistItem(int wishlistId, int wishlistItemId, CancellationToken cancellationToken);

    Task<WishlistDto?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken);

    Task<WishlistDto?> DeleteWishlistShare(int wishlistId, int wishlistShareId, CancellationToken cancellationToken);

    Task<WishlistShareDto?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken);

    Task<WishlistShareDto?> GetWishlistShareById(int id, CancellationToken cancellationToken);
}
