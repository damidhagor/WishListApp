using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Components.Modals;

namespace WishListApp.Components;

public partial class ShareComponent(
    IStringLocalizer<Localization> localizer,
    IJSRuntime jsRuntime,
    IAccessKeyGenerator accessKeyGenerator)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    [CascadingParameter]
    public WishListShare Share { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ConfirmationModalComponent _modal = default!;

    private string Url => _accessKeyGenerator.GenerateShareUrl(Share.AccessKey);

    private async Task CopyShareUrlToClipboard() => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);

    private async Task DeleteShare()
    {
        await _modal.Open(
            message: _localizer["ShareComponent_Delete_Message", Share.Name],
            confirmationCallback: async (confirmed) =>
            {
                if (confirmed)
                {
                    await ViewModel.DeleteWishListShare(Share, default);
                }
            });
    }
}
