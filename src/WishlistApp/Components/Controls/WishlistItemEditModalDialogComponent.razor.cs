using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.ProductCrawling.Services;
using WishlistApp.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistItemEditModalDialogComponent
{
    [Inject]
    private IProductCrawlerService ProductCrawlerService { get; set; } = default!;

    [Parameter, EditorRequired]
    public WishlistItemDto? WishlistItem { get; set; }

    private string _name = "";
    private string _description = "";
    private string _note = "";
    private string _price = "";

    protected override void OnParametersSet()
    {
        _name = WishlistItem?.Name ?? "";
        _description = WishlistItem?.Description ?? "";
        _note = WishlistItem?.Note ?? "";
        _price = WishlistItem?.Price ?? "";

        StateHasChanged();
    }
}