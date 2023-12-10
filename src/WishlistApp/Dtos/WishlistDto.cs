namespace WishlistApp.Dtos;

public sealed class WishlistDto
{
    public int Id { get; init; } = 0;

    public string OwnerIdentifier { get; set; } = "";

    public string Name { get; set; } = "";

    public WishlistItemDto[] Items { get; init; } = [];

    public WishlistShareDto[] Shares { get; init; } = [];
}
