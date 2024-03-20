using WishlistApp.Data.Models;

namespace WishlistApp.Data.Repositories;

public interface IWishlistShareRepository
{
    Task<WishlistShare?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken);

    Task<bool> DeleteWishlistShare(int shareId, CancellationToken cancellationToken);

    Task<WishlistShare?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken);

    Task<WishlistShare?> GetWishlistShareById(int shareId, CancellationToken cancellationToken);
}
