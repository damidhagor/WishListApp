using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Data.Sql;

public sealed class WishlistDbContext(DbContextOptions options) : DbContext(options)
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
        builder.Entity<Wishlist>()
            .Navigation(e => e.Shares)
            .AutoInclude();
        builder.Entity<Wishlist>()
            .Navigation(e => e.Items)
            .AutoInclude();

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
            .Navigation(e => e.Wishlist)
            .AutoInclude();
        builder.Entity<WishlistItem>()
            .Navigation(e => e.Purchases)
            .AutoInclude();

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
        builder.Entity<WishlistPurchase>()
            .Navigation(e => e.Item)
            .AutoInclude();
        builder.Entity<WishlistPurchase>()
            .Navigation(e => e.Share)
            .AutoInclude();

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
