namespace WishlistApp.Data.Models;

public sealed record WishlistItemPriority(
    WishlistItemPriorityValue Priority,
    string Name);