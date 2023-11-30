namespace WishlistApp.Dtos;

public sealed class WishlistShareDto
{
    public required int Id { get; init; }

    public required int WishlistId { get; init; }

    public required string Name { get; init; }

    public string AccessKey { get; set; } = "";

    public bool IsDeleted { get; set; }
}
