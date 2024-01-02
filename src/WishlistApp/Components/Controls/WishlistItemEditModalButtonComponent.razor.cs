using Microsoft.AspNetCore.Components;

namespace WishlistApp.Components.Controls;

public partial class WishlistItemEditModalButtonComponent
{
    [Parameter, EditorRequired]
    public WishlistItemDto? WishlistItem { get; set; }
}