using Microsoft.AspNetCore.Components;

namespace WishlistApp.Components.Controls;

public partial class WishlistItem
{
    [Parameter]
    public Data.Models.WishlistItem? Item { get; set; }

    [Parameter]
    public bool IsEditable { get; set; }
}