using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Models;

namespace WishlistApp.Data;

internal sealed class WishlistDbContext(DbContextOptions options) : IdentityDbContext<WishlistUser>(options)
{
    public DbSet<Wishlist> Wishlists { get; set; }

    public DbSet<WishlistItem> WishlistItems { get; set; }

    public DbSet<WishlistShare> WishlistShares { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Wishlist>()
            .HasKey(e => e.Id);
        builder.Entity<Wishlist>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Entity<WishlistItem>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistItem>()
            .Property(e => e.Url)
            .HasMaxLength(500)
            .IsRequired();
        builder.Entity<WishlistItem>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.WishlistId);
        builder.Entity<WishlistItem>()
            .HasOne(e => e.BoughtByWishlistShare)
            .WithMany(e => e.BoughtItems)
            .HasForeignKey(e => e.BoughtByWishlistShareId);

        builder.Entity<WishlistShare>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistShare>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Entity<WishlistShare>()
            .Property(e => e.AccessKey)
            .HasMaxLength(16)
            .IsRequired();
        builder.Entity<WishlistShare>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Shares)
            .HasForeignKey(e => e.WishlistId);
    }
}
