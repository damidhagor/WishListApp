using WishlistApp.Data.Sql.Models;

namespace WishlistApp.Dtos;

public sealed record WishlistItemPriorityDto(
    WishlistItemPriority Priority,
    string Name);