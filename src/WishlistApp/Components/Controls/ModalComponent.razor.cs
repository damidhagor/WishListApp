using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WishlistApp.Components.Controls;

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

    public async Task Open()
    {
        await JSRuntime.InvokeVoidAsync("OpenModal", $"#{Id}");
    }

    public async Task Close()
    {
        await JSRuntime.InvokeVoidAsync("CloseModal", $"#{Id}");
    }
}