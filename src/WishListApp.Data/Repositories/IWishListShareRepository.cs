namespace WishListApp.Data.Repositories;

public interface IWishListShareRepository
{
    Task<WishListShare> Add(ObjectId wishListId, string shareName, string shareAccessKey, CancellationToken cancellationToken);

    Task<WishListShare?> GetById(ObjectId shareId, CancellationToken cancellationToken);

    Task<WishListShare?> GetByAccessKey(string accessKey, CancellationToken cancellationToken);

    Task<bool> Delete(ObjectId shareId, CancellationToken cancellationToken);
}
