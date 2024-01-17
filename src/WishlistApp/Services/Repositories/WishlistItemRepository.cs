using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;
using WishlistApp.Data.Models;

namespace WishlistApp.Services.Repositories;

internal sealed class WishlistItemRepository(WishlistDbContext wishlistDbContext) : IWishlistItemRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

    public async Task<WishlistItemDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNullOrWhiteSpace(url, nameof(url));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = new WishlistItem
            {
                WishlistId = wishlistId,
                Wishlist = wishlist,
                Url = url,
            };

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto updatedItem, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(updatedItem, nameof(updatedItem));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == updatedItem.Id, cancellationToken);

        if (item is not null)
        {
            item.Name = updatedItem.Name;
            item.Description = updatedItem.Description;
            item.Note = updatedItem.Note;
            item.Price = updatedItem.Price;
            item.Priority = updatedItem.Priority.Priority;

            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<bool> DeleteWishlistItem(int itemid, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemid, -1, nameof(itemid));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == itemid, cancellationToken);

        if (item is not null)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task DeleteBoughtWishlistItems(int wishlistId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var items = _context.WishlistItems
            .Where(i => i.WishlistId == wishlistId
                     && i.BoughtByWishlistShareId != null);

        _context.WishlistItems.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<WishlistItemDto?> BuyWishlistItem(int itemId, int shareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemId, -1, nameof(itemId));
        Guard.IsGreaterThan(shareId, -1, nameof(shareId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item is not null)
        {
            item.BoughtByWishlistShareId = shareId;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> UnbuyWishlistItem(int itemId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemId, -1, nameof(itemId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item is not null)
        {
            item.BoughtByWishlistShareId = null;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> SetWishlistItemPriority(int itemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemId, -1, nameof(itemId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item is not null)
        {
            item.Priority = priority.Priority;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }
}
