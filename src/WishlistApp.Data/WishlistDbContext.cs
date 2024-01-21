using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Models;

namespace WishlistApp.Data;

public sealed class WishlistDbContext(DbContextOptions options) : IdentityDbContext<WishlistUser, WishlistRole, string>(options)
{
    public DbSet<Wishlist> Wishlists { get; set; }

    public DbSet<WishlistItem> Items { get; set; }

    public DbSet<WishlistShare> Shares { get; set; }

    public DbSet<WishlistPurchase> Purchases { get; set; }

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

        builder.Entity<WishlistPurchase>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistPurchase>()
            .HasOne(e => e.Item)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ItemId);
        builder.Entity<WishlistPurchase>()
            .HasOne(e => e.Share)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ShareId);

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
