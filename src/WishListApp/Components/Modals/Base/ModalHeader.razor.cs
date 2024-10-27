using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals.Base;

public sealed partial class ModalHeader
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;
}
