namespace WishlistApp.Services.Repositories;

public interface IWishlistShareRepository
{
    Task<WishlistShareDto?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistShare(int wishlistShareId, CancellationToken cancellationToken);

    Task<WishlistShareDto?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken);

    Task<WishlistShareDto?> GetWishlistShareById(int id, CancellationToken cancellationToken);
}
