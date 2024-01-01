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

    [Parameter, EditorRequired]
    public string? ModalId { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemUpdated { get; set; }

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
    }

    private async Task LoadCrawledProductInformation()
    {
        if (WishlistItem is null)
        {
            return;
        }

        var info = await ProductCrawlerService.CrawlProduct(new Uri(WishlistItem.Url), default);

        _name = string.IsNullOrWhiteSpace(info.Title) ? _name : info.Title;
        _description = string.IsNullOrWhiteSpace(info.Description) ? _description : info.Description;
        _price = string.IsNullOrWhiteSpace(info.Price) && string.IsNullOrWhiteSpace(info.Currency)
            ? _price
            : $"{info.Price}{info.Currency}";
    }

    private async Task OnItemUpdated()
    {
        if (WishlistItem is null)
        {
            return;
        }

        await ItemUpdated.InvokeAsync(WishlistItem with
        {
            Name = _name,
            Description = _description,
            Price = _price
        });
    }
}