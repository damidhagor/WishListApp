namespace WishListApp.Data.Repositories;

public interface IWishListItemRepository
{
    Task<ObjectId> Add(ObjectId wishListId, string url, CancellationToken cancellationToken);

    Task<WishList?> Delete(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken);

    Task<WishList?> DeletePurchasedItems(ObjectId wishListId, CancellationToken cancellationToken);

    Task<WishList?> Update(ObjectId wishListId, WishListItem item, CancellationToken cancellationToken);

    Task<WishList?> UpdatePriority(ObjectId wishListId, ObjectId itemId, int priority, CancellationToken cancellationToken);

    Task<WishList?> UpdatePurchaseQuantity(ObjectId wishListId, ObjectId itemId, ObjectId shareId, int quantity, CancellationToken cancellationToken);

    Task<WishList?> ResetPurchases(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken);

    Task<WishList?> MoveToWishList(ObjectId wishListId, ObjectId itemId, ObjectId newWishListId, CancellationToken cancellationToken);
}
