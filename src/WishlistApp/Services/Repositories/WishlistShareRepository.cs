using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data;
using WishlistApp.Data.Models;

namespace WishlistApp.Services.Repositories;

internal sealed class WishlistShareRepository(WishlistDbContext wishlistDbContext, IAccessKeyGenerator accessKeyGenerator) : IWishlistShareRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    public async Task<WishlistShareDto?> AddWishlistShare(int wishlistId, string name, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        var wishlist = await _context.Wishlists
            .Include(w => w.Shares)
            .FirstOrDefaultAsync(w => w.Id == wishlistId, cancellationToken);

        if (wishlist is null)
        {
            return null;
        }

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

        return newShare.ToDto();
    }

    public async Task<bool> DeleteWishlistShare(int wishlistShareId, CancellationToken cancellationToken)
    {
        var share = _context.WishlistShares.FirstOrDefault(i => i.Id == wishlistShareId);

        if (share is not null)
        {
            share.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<WishlistShareDto?> GetWishlistShareByAccessKey(string accessKey, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(accessKey, nameof(accessKey));

        var share = await _context.WishlistShares.FirstOrDefaultAsync(s => s.AccessKey == accessKey, cancellationToken);
        return share?.ToDto();
    }

    public async Task<WishlistShareDto?> GetWishlistShareById(int id, CancellationToken cancellationToken)
    {
        Guard.IsGreaterThan(id, -1, nameof(id));

        var share = await _context.WishlistShares.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return share?.ToDto();
    }
}
