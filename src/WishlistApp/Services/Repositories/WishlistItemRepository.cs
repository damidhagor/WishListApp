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

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = new WishlistItem
            {
                WishlistId = wishlistId,
                Wishlist = wishlist,
                Url = url,
                Quantity = 1
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto updatedItem, CancellationToken cancellationToken)
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
            return item.ToDto();
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

    public async Task<WishlistItemDto?> SetWishlistItemPriority(int itemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(itemId, -1, nameof(itemId));

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item is not null)
        {
            item.Priority = priority.Priority;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }
}
