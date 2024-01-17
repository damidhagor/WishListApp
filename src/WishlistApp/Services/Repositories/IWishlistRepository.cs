namespace WishlistApp.Services.Repositories;

public interface IWishlistRepository
{
    Task<WishlistDto> CreateWishlist(string name, string ownerIdentifier, CancellationToken cancellationToken);

    Task<WishlistDto?> RenameWishlist(int id, string name, CancellationToken cancellationToken);

    Task<WishlistDto?> GetWishlist(int id, CancellationToken cancellationToken);

    Task<List<WishlistDto>> GetAll(string ownerIdentifier, CancellationToken cancellationToken);

    Task<bool> DeleteWishlist(int id, CancellationToken cancellationToken);
}
