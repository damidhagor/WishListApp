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

    public async Task<WishlistDto?> RenameWishlist(int wishlistId, string name, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            wishlist.Name = name;
            await _context.SaveChangesAsync(cancellationToken);
            return wishlist.ToDto();
        }

        return null;
    }

    public async Task<WishlistDto?> GetWishlist(int wishlistId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        return wishlist?.ToDto();
    }

    public async Task<List<WishlistDto>> GetAll(string ownerIdentifier, CancellationToken cancellationToken)
    {
        var wishlists = await _context.Wishlists
            .Where(w => w.OwnerIdentifier == ownerIdentifier)
            .ToArrayAsync(cancellationToken);

        return wishlists.ToDtos().ToList();
    }

    public async Task<bool> DeleteWishlist(int wishlistId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
