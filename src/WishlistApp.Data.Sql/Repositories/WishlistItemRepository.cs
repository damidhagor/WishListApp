using WishlistApp.Data.Models;
using WishlistApp.Data.Repositories;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Data.Sql.Repositories;

internal sealed class WishlistItemRepository(WishlistDbContext wishlistDbContext) : IWishlistItemRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

    public async Task<WishlistItem?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNullOrWhiteSpace(url, nameof(url));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = new WishlistItemEntity
            {
                WishlistId = wishlistId,
                Wishlist = wishlist,
                Url = url,
                Quantity = 1
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToModel();
        }

        return null;
    }

    public async Task<WishlistItem?> UpdateWishlistItem(WishlistItem updatedItem, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(updatedItem, nameof(updatedItem));

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == updatedItem.Id, cancellationToken);

        if (item is not null)
        {
            item.Name = updatedItem.Name;
            item.Description = updatedItem.Description;
            item.Note = updatedItem.Note;
            item.Price = updatedItem.Price;
            item.Quantity = updatedItem.Quantity;
            item.Priority = updatedItem.Priority.Priority;

            await _context.SaveChangesAsync(cancellationToken);
            return item.ToModel();
        }

        return null;
    }

    public async Task<bool> DeleteWishlistItem(int itemid, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemid, -1, nameof(itemid));

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemid, cancellationToken);

        if (item is not null)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task DeletePurchasedWishlistItems(int wishlistId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var items = _context.Items
            .Where(i => i.WishlistId == wishlistId
                     && i.Purchases.Sum(b => b.Quantity) >= i.Quantity);

        _context.Items.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<WishlistItem?> SetWishlistItemPriority(int itemId, WishlistItemPriority priority, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemId, -1, nameof(itemId));

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item is not null)
        {
            item.Priority = priority.Priority;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToModel();
        }

        return null;
    }
}
