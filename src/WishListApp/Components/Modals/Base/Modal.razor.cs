using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals.Base;

public sealed partial class Modal
{
    [Parameter, EditorRequired]
    public string Id { get; set; } = default!;

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public bool AllowFullscreen { get; set; }

    private string _fullscreenClass => AllowFullscreen ? "modal-fullscreen-md-down" : string.Empty;
}
