using WishlistApp.Data.Models;
using WishlistApp.Data.Repositories;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Data.Sql.Repositories;

internal sealed class WishlistRepository(WishlistDbContext wishlistDbContext) : IWishlistRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

    public async Task<Wishlist> CreateWishlist(string name, string ownerIdentifier, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = new WishlistEntity { Name = name, OwnerIdentifier = ownerIdentifier };
        _context.Wishlists.Add(wishlist);

        await _context.SaveChangesAsync(cancellationToken);
        return wishlist.ToModel();
    }

    public async Task<Wishlist?> RenameWishlist(int wishlistId, string name, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is not null)
        {
            wishlist.Name = name;
            await _context.SaveChangesAsync(cancellationToken);
            return wishlist.ToModel();
        }

        return null;
    }

    public async Task<Wishlist?> GetWishlist(int wishlistId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(wishlistId, -1, nameof(wishlistId));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        return wishlist?.ToModel();
    }

    public async Task<List<Wishlist>> GetAll(string ownerIdentifier, CancellationToken cancellationToken)
    {
        var wishlists = await _context.Wishlists
            .Where(w => w.OwnerIdentifier == ownerIdentifier)
            .ToArrayAsync(cancellationToken);

        return wishlists.ToModels().ToList();
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
