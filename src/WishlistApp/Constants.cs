using WishlistApp.Data.Sql.Models;

namespace WishlistApp;

internal static class Constants
{
    public readonly static WishlistItemPriorityDto[] WishlistItemPriorities =
        [
            WishlistItemPriority.Unknown.ToDto(),
            WishlistItemPriority.Low.ToDto(),
            WishlistItemPriority.Medium.ToDto(),
            WishlistItemPriority.High.ToDto(),
            WishlistItemPriority.VeryHigh.ToDto()
        ];
}
