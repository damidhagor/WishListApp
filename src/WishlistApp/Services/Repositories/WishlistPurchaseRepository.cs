using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Sql;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Services.Repositories;

internal sealed class WishlistPurchaseRepository(WishlistDbContext wishlistDbContext) : IWishlistPurchaseRepository
{
    private readonly WishlistDbContext _context = wishlistDbContext;

    public async Task<WishlistPurchaseDto?> UpdatePurchaseQuantity(int itemId, int shareId, int quantity, CancellationToken cancellationToken)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
        var share = await _context.Shares.FirstOrDefaultAsync(s => s.Id == shareId, cancellationToken);
        var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.ItemId == itemId && p.ShareId == shareId, cancellationToken);

        if ((item is null || share is null) && purchase is not null)
        {
            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync(cancellationToken);
            return null;
        }

        if (item is null || share is null)
        {
            return null;
        }

        if (purchase is null && quantity > 0)
        {
            purchase = new WishlistPurchase
            {
                ItemId = itemId,
                Item = item,
                ShareId = shareId,
                Share = share,
                Quantity = quantity
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync(cancellationToken);
            return purchase.ToDto();
        }

        if (purchase is not null)
        {
            purchase.Quantity = Math.Max(0, purchase.Quantity + quantity);

            if (purchase.Quantity <= 0)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return purchase.ToDto();
        }

        return null;
    }
}
