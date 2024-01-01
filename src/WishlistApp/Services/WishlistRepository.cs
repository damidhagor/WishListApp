using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;
using WishlistApp.Data.Models;

namespace WishlistApp.Services;

internal sealed class WishlistRepository(WishlistDbContext wishlistDbContext, IAccessKeyGenerator accessKeyGenerator) : IWishlistRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    public async Task<WishlistDto> CreateWishlist(string name, string ownerIdentifier, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = new Wishlist { Name = name, OwnerIdentifier = ownerIdentifier };
        _context.Wishlists.Add(wishlist);

        await _context.SaveChangesAsync(cancellationToken);
        return wishlist.ToDto();
    }

    public async Task<WishlistDto?> RenameWishlist(int id, string name, CancellationToken cancellationToken)
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

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> GetWishlist(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return wishlist?.ToDto();
    }

    public async Task<List<WishlistDto>> GetAll(string ownerIdentifier, CancellationToken cancellationToken)
    {
        var wishlists = await _context.Wishlists
            .Where(w => w.OwnerIdentifier == ownerIdentifier)
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .ToArrayAsync(cancellationToken);

        return wishlists.ToDtos().ToList();
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

    public async Task<WishlistDto?> AddWishlistItem(int wishlistId, string url, CancellationToken cancellationToken)
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

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> UpdateWishlistItem(int wishlistId, WishlistItemDto wishlistItemDto, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNull(wishlistItemDto, nameof(wishlistItemDto));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = wishlist.Items.FirstOrDefault(i => i.Id == wishlistItemDto.Id);
            if (item is not null)
            {
                item.Name = wishlistItemDto.Name;
                item.Description = wishlistItemDto.Description;
                item.Note = wishlistItemDto.Note;
                item.Price = wishlistItemDto.Price;
                item.Priority = wishlistItemDto.Priority.Priority;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> DeleteWishlistItem(int wishlistId, int wishlistItemId, CancellationToken cancellationToken)
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

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> DeleteWishlistItems(int wishlistId, int[] wishlistItemIds, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            foreach (var wishlistItemId in wishlistItemIds)
            {
                var item = wishlist.Items.FirstOrDefault(i => i.Id == wishlistItemId);
                if (item is not null)
                {
                    wishlist.Items.Remove(item);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> BuyWishlistItem(int wishlistId, int wishlistItemId, int wishlistShareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));
        Guard.IsGreaterThan(wishlistShareId, -1, nameof(wishlistShareId));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = wishlist.Items.FirstOrDefault(i => i.Id == wishlistItemId);
            if (item is not null)
            {
                item.BoughtByWishlistShareId = wishlistShareId;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> UnbuyWishlistItem(int wishlistId, int wishlistItemId, int wishlistShareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsGreaterThan(wishlistItemId, -1, nameof(wishlistItemId));
        Guard.IsGreaterThan(wishlistShareId, -1, nameof(wishlistShareId));

        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            var item = wishlist.Items.FirstOrDefault(i => i.Id == wishlistItemId);
            if (item is not null)
            {
                item.BoughtByWishlistShareId = null;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> SetWishlistItemPriority(int wishlistId, int wishlistItemId, WishlistItemPriority priority, CancellationToken cancellationToken)
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
                item.Priority = priority;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken)
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

        return wishlist?.ToDto();
    }

    public async Task<WishlistDto?> DeleteWishlistShare(int wishlistId, int wishlistShareId, CancellationToken cancellationToken)
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

        return wishlist?.ToDto();
    }

    public async Task<WishlistShareDto?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(accessKey, nameof(accessKey));

        var share = await _context.WishlistShares
            .FirstOrDefaultAsync(s => s.AccessKey == accessKey, cancellationToken);
        return share?.ToDto();
    }

    public async Task<WishlistShareDto?> GetWishlistShareById(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var share = await _context.WishlistShares
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return share?.ToDto();
    }
}
