using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Data.Sql;

public sealed class WishlistDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<WishlistEntity> Wishlists { get; set; }

    public DbSet<WishlistItemEntity> Items { get; set; }

    public DbSet<WishlistShareEntity> Shares { get; set; }

    public DbSet<WishlistPurchaseEntity> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WishlistEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistEntity>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Entity<WishlistEntity>()
            .Navigation(e => e.Shares)
            .AutoInclude();
        builder.Entity<WishlistEntity>()
            .Navigation(e => e.Items)
            .AutoInclude();

        builder.Entity<WishlistItemEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistItemEntity>()
            .Property(e => e.Url)
            .HasMaxLength(500)
            .IsRequired();
        builder.Entity<WishlistItemEntity>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.WishlistId);
        builder.Entity<WishlistItemEntity>()
            .Navigation(e => e.Wishlist)
            .AutoInclude();
        builder.Entity<WishlistItemEntity>()
            .Navigation(e => e.Purchases)
            .AutoInclude();

        builder.Entity<WishlistPurchaseEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistPurchaseEntity>()
            .HasOne(e => e.Item)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ItemId);
        builder.Entity<WishlistPurchaseEntity>()
            .HasOne(e => e.Share)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ShareId);
        builder.Entity<WishlistPurchaseEntity>()
            .Navigation(e => e.Item)
            .AutoInclude();
        builder.Entity<WishlistPurchaseEntity>()
            .Navigation(e => e.Share)
            .AutoInclude();

        builder.Entity<WishlistShareEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishlistShareEntity>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Entity<WishlistShareEntity>()
            .Property(e => e.AccessKey)
            .HasMaxLength(16)
            .IsRequired();
        builder.Entity<WishlistShareEntity>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Shares)
            .HasForeignKey(e => e.WishlistId);
    }
}
