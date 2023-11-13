using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;
using WishlistApp.Data.Models;

namespace WishlistApp.Services;

internal sealed class WishlistRepository(WishlistDbContext wishlistDbContext) : IWishlistRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

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

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
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

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return wishlist;
    }

    public async Task<List<Wishlist>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Wishlists.ToListAsync(cancellationToken);
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
}
