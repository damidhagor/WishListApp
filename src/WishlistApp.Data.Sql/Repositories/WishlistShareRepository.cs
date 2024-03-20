using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Models;
using WishlistApp.Data.Repositories;
using WishlistApp.Data.Services;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Data.Sql.Repositories;

internal sealed class WishlistShareRepository(WishlistDbContext wishlistDbContext, IAccessKeyGenerator accessKeyGenerator) : IWishlistShareRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    public async Task<WishlistShare?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is null)
        {
            return null;
        }

        var accessKey = _accessKeyGenerator.GenerateAccessKey(16);
        var share = new WishlistShareEntity
        {
            WishlistId = wishlistId,
            Wishlist = wishlist,
            Name = name,
            AccessKey = accessKey
        };

        _context.Shares.Add(share);
        await _context.SaveChangesAsync(cancellationToken);

        return share.ToModel();
    }

    public async Task<bool> DeleteWishlistShare(int shareId, CancellationToken cancellationToken)
    {
        var share = _context.Shares.FirstOrDefault(i => i.Id == shareId);

        if (share is not null)
        {
            share.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<WishlistShare?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(accessKey, nameof(accessKey));

        var share = await _context.Shares.FirstOrDefaultAsync(s => s.AccessKey == accessKey, cancellationToken);
        return share?.ToModel();
    }

    public async Task<WishlistShare?> GetWishlistShareById(int shareId, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(shareId, -1, nameof(shareId));

        var share = await _context.Shares.FirstOrDefaultAsync(s => s.Id == shareId, cancellationToken);
        return share?.ToModel();
    }
}
