using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals.Base;

public sealed partial class ModalFooter
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
