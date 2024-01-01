using Microsoft.AspNetCore.Components;

namespace WishlistApp.Components.Controls;

public partial class WishlistItemEditModalButtonComponent
{
    [Parameter, EditorRequired]
    public string? ModalId { get; set; }
}