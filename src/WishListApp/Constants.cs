using WishListApp.Data.Models;

namespace WishListApp;

internal static class Constants
{
    public readonly static WishlistItemPriority[] WishlistItemPriorities =
        [
            WishlistItemPriorityValue.Unknown.ToModel(),
            WishlistItemPriorityValue.Low.ToModel(),
            WishlistItemPriorityValue.Medium.ToModel(),
            WishlistItemPriorityValue.High.ToModel(),
            WishlistItemPriorityValue.VeryHigh.ToModel()
        ];
}
