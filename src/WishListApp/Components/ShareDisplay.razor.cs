using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Models.Modals.Results;
using WishListApp.Services;

namespace WishListApp.Components;

public sealed partial class ShareDisplay(
    IJSRuntime jsRuntime,
    IModalService modalService,
    IAccessKeyGenerator accessKeyGenerator)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly IModalService _modalService = modalService;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

    [CascadingParameter]
    public WishListShare Share { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private string Url => _accessKeyGenerator.GenerateShareUrl(Share.AccessKey);

    private async Task CopyShareUrlToClipboard() => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);

    private async Task DeleteShare()
    {
        try
        {
            var result = await _modalService.ShowConfirmation(string.Format(_localization.ShareComponent_Delete_Message, Share.Name));
            if (result.IsConfirmed())
            {
                await ViewModel.DeleteWishListShare(Share, default);
            }
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ShareDelete, exception: e);
        }
    }
}
