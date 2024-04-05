namespace WishListApp.Data.Repositories;

public interface IWishListRepository
{
    Task<WishList> Add(string name, string ownerId, CancellationToken cancellationToken);

    Task<WishList?> GetById(ObjectId wishListId, CancellationToken cancellationToken);

    Task<List<WishList>> GetByOwner(string ownerId, CancellationToken cancellationToken);

    Task<WishList?> Rename(ObjectId wishListId, string name, CancellationToken cancellationToken);

    Task<bool> Delete(ObjectId wishListId, CancellationToken cancellationToken);
}
