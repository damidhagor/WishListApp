using Microsoft.AspNetCore.Components;
using WishlistApp.Data.Models;

namespace WishlistApp.Components.Controls;

public partial class WishlistItemPriorityDropdownComponent
{
    [Parameter, EditorRequired]
    public WishlistItemDto? WishlistItem { get; set; }

    [Parameter]
    public EventCallback<WishlistItemPriority> PriorityChanged { get; set; }

    private WishlistItemPriorityDto[] Priorities { get; set; } =
        [
            WishlistItemPriority.Unknown.ToDto(),
            WishlistItemPriority.Low.ToDto(),
            WishlistItemPriority.Medium.ToDto(),
            WishlistItemPriority.High.ToDto(),
            WishlistItemPriority.VeryHigh.ToDto()
        ];
}