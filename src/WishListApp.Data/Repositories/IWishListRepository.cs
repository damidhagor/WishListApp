namespace WishListApp.Data.Repositories;

public interface IWishListRepository
{
    Task<WishList> CreateWishList(string name, string ownerId, CancellationToken cancellationToken);

    Task<WishList?> GetWishListById(string wishListId, CancellationToken cancellationToken);

    Task<List<WishList>> GetWishListsForOwner(string ownerId, CancellationToken cancellationToken);

    Task<WishList?> RenameWishList(string wishListId, string name, CancellationToken cancellationToken);

    Task<bool> DeleteWishList(string wishListId, CancellationToken cancellationToken);
}
