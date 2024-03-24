using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WishListApp.Components.Controls.Modals;

public partial class ModalComponent
{
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

    private string _sizeClass => GetSizeClass();

    public async Task Open()
    {
        await JSRuntime.InvokeVoidAsync("OpenModal", $"#{Id}");
    }

    public async Task Close()
    {
        await JSRuntime.InvokeVoidAsync("CloseModal", $"#{Id}");
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