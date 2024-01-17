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
            var newItem = new WishlistItem
            {
                WishlistId = wishlistId,
                Wishlist = wishlist,
                Url = url,
            };

            _context.WishlistItems.Add(newItem);
            await _context.SaveChangesAsync(cancellationToken);
            return newItem.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> UpdateWishlistItem(WishlistItemDto wishlistItemDto, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(wishlistItemDto, nameof(wishlistItemDto));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == wishlistItemDto.Id, cancellationToken);

        if (item is not null)
        {
            item.Name = wishlistItemDto.Name;
            item.Description = wishlistItemDto.Description;
            item.Note = wishlistItemDto.Note;
            item.Price = wishlistItemDto.Price;
            item.Priority = wishlistItemDto.Priority.Priority;

            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<bool> DeleteWishlistItem(int wishlistItemId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == wishlistItemId, cancellationToken);

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

    public async Task<WishlistItemDto?> BuyWishlistItem(int wishlistItemId, int wishlistShareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));
        Guard.IsGreaterThan(wishlistShareId, -1, nameof(wishlistShareId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == wishlistItemId, cancellationToken);

        if (item is not null)
        {
            item.BoughtByWishlistShareId = wishlistShareId;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> UnbuyWishlistItem(int wishlistItemId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == wishlistItemId, cancellationToken);

        if (item is not null)
        {
            item.BoughtByWishlistShareId = null;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }

    public async Task<WishlistItemDto?> SetWishlistItemPriority(int wishlistItemId, WishlistItemPriorityDto priority, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));

        var item = await _context.WishlistItems.FirstOrDefaultAsync(i => i.Id == wishlistItemId, cancellationToken);

        if (item is not null)
        {
            item.Priority = priority.Priority;
            await _context.SaveChangesAsync(cancellationToken);
            return item.ToDto();
        }

        return null;
    }
}
