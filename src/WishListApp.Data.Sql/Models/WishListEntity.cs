namespace WishListApp.Data.Sql.Models;

public sealed class WishListEntity
{
    public int Id { get; set; } = 0;

    public string OwnerIdentifier { get; set; } = "";

    public string Name { get; set; } = "";

    public List<WishListItemEntity> Items { get; set; } = [];

    public List<WishListShareEntity> Shares { get; set; } = [];
}
