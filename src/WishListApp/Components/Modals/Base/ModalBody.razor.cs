using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals.Base;

public sealed partial class ModalBody
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
