namespace WishListApp.Data.Repositories;

public interface IWishListItemRepository
{
    Task<WishList?> Add(ObjectId wishListId, string url, CancellationToken cancellationToken);

    Task<WishList?> Delete(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken);

    Task<WishList?> Update(ObjectId wishListId, WishListItem item, CancellationToken cancellationToken);

    Task<WishList?> UpdatePurchaseQuantity(ObjectId wishListId, ObjectId itemId, ObjectId shareId, int quantity, CancellationToken cancellationToken);
}
