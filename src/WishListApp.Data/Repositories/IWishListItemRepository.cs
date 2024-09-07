namespace WishListApp.Data.Repositories;

public interface IWishListItemRepository
{
    Task<ObjectId?> Add(ObjectId wishListId, string url, CancellationToken cancellationToken);

    Task<WishList?> Delete(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken);

    Task<WishList?> DeletePurchasedItems(ObjectId wishListId, CancellationToken cancellationToken);

    Task<WishList?> Update(ObjectId wishListId, WishListItem item, CancellationToken cancellationToken);

    Task<WishList?> UpdatePriority(ObjectId wishListId, ObjectId itemId, int priority, CancellationToken cancellationToken);

    Task<WishList?> SetPurchaser(ObjectId wishListId, ObjectId itemId, ObjectId? purchaserId, CancellationToken cancellationToken);

    Task<ObjectId?> MoveToWishList(ObjectId wishListId, ObjectId itemId, ObjectId newWishListId, CancellationToken cancellationToken);
}
