using Microsoft.EntityFrameworkCore;
using WishListApp.Data.Sql.Models;

namespace WishListApp.Data.Sql;

public sealed class WishListDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<WishListEntity> Wishlists { get; set; }

    public DbSet<WishListItemEntity> Items { get; set; }

    public DbSet<WishListShareEntity> Shares { get; set; }

    public DbSet<WishListPurchaseEntity> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WishListEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishListEntity>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Entity<WishListEntity>()
            .Navigation(e => e.Shares)
            .AutoInclude();
        builder.Entity<WishListEntity>()
            .Navigation(e => e.Items)
            .AutoInclude();

        builder.Entity<WishListItemEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishListItemEntity>()
            .Property(e => e.Url)
            .HasMaxLength(500)
            .IsRequired();
        builder.Entity<WishListItemEntity>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.WishlistId);
        builder.Entity<WishListItemEntity>()
            .Navigation(e => e.Wishlist)
            .AutoInclude();
        builder.Entity<WishListItemEntity>()
            .Navigation(e => e.Purchases)
            .AutoInclude();

        builder.Entity<WishListPurchaseEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishListPurchaseEntity>()
            .HasOne(e => e.Item)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ItemId);
        builder.Entity<WishListPurchaseEntity>()
            .HasOne(e => e.Share)
            .WithMany(e => e.Purchases)
            .HasForeignKey(e => e.ShareId);
        builder.Entity<WishListPurchaseEntity>()
            .Navigation(e => e.Item)
            .AutoInclude();
        builder.Entity<WishListPurchaseEntity>()
            .Navigation(e => e.Share)
            .AutoInclude();

        builder.Entity<WishListShareEntity>()
            .HasKey(e => e.Id);
        builder.Entity<WishListShareEntity>()
            .Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Entity<WishListShareEntity>()
            .Property(e => e.AccessKey)
            .HasMaxLength(16)
            .IsRequired();
        builder.Entity<WishListShareEntity>()
            .HasOne(e => e.Wishlist)
            .WithMany(e => e.Shares)
            .HasForeignKey(e => e.WishlistId);
    }
}
