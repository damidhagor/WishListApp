namespace WishlistApp.Data.Models;

public static class MappingExtensions
{
    public static WishlistItemPriority ToModel(this WishlistItemPriorityValue priority)
        => new(priority, priority switch
        {
            WishlistItemPriorityValue.Low => "Nicht unbedingt",
            WishlistItemPriorityValue.Medium => "Hätte ich gerne",
            WishlistItemPriorityValue.High => "Hätte ich sehr gerne",
            WishlistItemPriorityValue.VeryHigh => "Muss ich haben",
            _ => "Unbekannt"
        });
}