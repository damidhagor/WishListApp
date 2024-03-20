namespace WishlistApp.Data.Sql.Models;

public sealed class Wishlist
{
    public int Id { get; set; } = 0;

    public string OwnerIdentifier { get; set; } = "";

    public string Name { get; set; } = "";

    public List<WishlistItem> Items { get; set; } = [];

    public List<WishlistShare> Shares { get; set; } = [];
}
