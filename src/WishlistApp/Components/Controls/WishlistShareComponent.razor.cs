using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistShareComponent
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IAccessKeyGenerator AccessKeyGenerator { get; set; } = default!;

    [Parameter, EditorRequired]
    public WishlistShareDto? Share { get; set; }

    [Parameter]
    public EventCallback<WishlistShareDto> ShareDeleted { get; set; }

    private string Url => Share is not null ? AccessKeyGenerator.GenerateWishlistShareUrl(Share.AccessKey) : "";

    private async Task CopyShareUrlToClipboard()
    {
        if (Share is null)
        {
            return;
        }

        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);
    }
}