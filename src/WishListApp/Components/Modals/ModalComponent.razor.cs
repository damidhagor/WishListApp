using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WishListApp.Components.Modals;

public partial class ModalComponent(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    [Parameter]
    public string Id { get; set; } = $"id{Guid.NewGuid()}";

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter]
    public RenderFragment? Body { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }

    [Parameter]
    public ModalSize Size { get; set; } = ModalSize.Default;

    [Parameter]
    public string? ModalDialogClass { get; set; }

    private string _sizeClass => GetSizeClass();

    public async Task Open()
    {
        await _jsRuntime.InvokeVoidAsync("OpenModal", $"#{Id}");
    }

    public async Task Close()
    {
        await _jsRuntime.InvokeVoidAsync("CloseModal", $"#{Id}");
    }

    private string GetSizeClass()
        => Size switch
        {
            ModalSize.Small => "modal-sm",
            ModalSize.Large => "modal-lg",
            ModalSize.ExtraLarge => "modal-xl",
            _ => ""
        };

    public enum ModalSize
    {
        Default,
        Small,
        Large,
        ExtraLarge
    }
}
