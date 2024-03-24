namespace WishlistApp.Data.Sql.Models;

public sealed class WishlistEntity
{
    public int Id { get; set; } = 0;

    public string OwnerIdentifier { get; set; } = "";

    public string Name { get; set; } = "";

    public List<WishlistItemEntity> Items { get; set; } = [];

    public List<WishlistShareEntity> Shares { get; set; } = [];
}
