using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;
using WishlistApp.Data.Models;

namespace WishlistApp.Services.Repositories;

internal sealed class WishlistRepository(WishlistDbContext wishlistDbContext) : IWishlistRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

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
            return wishlist.ToDto();
        }

        return null;
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

    public async Task<bool> DeleteWishlist(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (wishlist is not null)
        {
            _context.Wishlists.Remove(wishlist);
            _context.WishlistItems.RemoveRange(wishlist.Items);
            _context.WishlistShares.RemoveRange(wishlist.Shares);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
