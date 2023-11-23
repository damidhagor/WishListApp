using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;

namespace WishlistApp.Services;

internal sealed class WishlistRepository(WishlistDbContext wishlistDbContext, IAccessKeyGenerator accessKeyGenerator) : IWishlistRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    public async Task<Wishlist> CreateWishlist(string name, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = new Wishlist { Name = name };
        _context.Wishlists.Add(wishlist);

        await _context.SaveChangesAsync(cancellationToken);
        return wishlist;
    }

    public async Task<Wishlist?> RenameWishlist(int id, string name, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (wishlist is not null)
        {
            wishlist.Name = name;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return wishlist;
    }

    public async Task<Wishlist?> GetWishlist(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return wishlist;
    }

    public async Task<List<Wishlist>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteWishlist(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (wishlist is not null)
        {
            _context.Wishlists.Remove(wishlist);
            _context.WishlistItems.RemoveRange(wishlist.Items);
            _context.WishlistShares.RemoveRange(wishlist.Shares);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Wishlist?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken)
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
        }

        return wishlist;
    }

    public async Task<Wishlist?> DeleteWishlistItem(int wishlistId, int wishlistItemId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = wishlist.Items.FirstOrDefault(i => i.Id == wishlistItemId);
            if (item is not null)
            {
                wishlist.Items.Remove(item);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist;
    }

    public async Task<Wishlist?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var accessKey = _accessKeyGenerator.GenerateAccessKey(16);
            var newShare = new WishlistShare
            {
                WishlistId = wishlistId,
                Wishlist = wishlist,
                Name = name,
                AccessKey = accessKey
            };

            _context.WishlistShares.Add(newShare);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return wishlist;
    }

    public async Task<Wishlist?> DeleteWishlistShare(int wishlistId, int wishlistShareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsGreaterThan(wishlistShareId, -1, nameof(wishlistShareId));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var share = wishlist.Shares.FirstOrDefault(i => i.Id == wishlistShareId);
            if (share is not null)
            {
                share.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist;
    }
}
