using WishlistApp.Data.Models;

namespace WishlistApp.Dtos;

public sealed record WishlistItemPriorityDto(WishlistItemPriority Priority, string Name);