namespace WishlistApp.Dtos;

public sealed record WishlistShareDto(
    int Id,
    int WishlistId,
    string Name,
    string AccessKey,
    bool IsDeleted);
