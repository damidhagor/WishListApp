using Microsoft.AspNetCore.Components;
using WishlistApp.Data.Models;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistItemEditModalComponent
{
    [Inject]
    private IProductCrawlerService ProductCrawlerService { get; set; } = default!;

    [Parameter]
    public EventCallback<WishlistItemDto> ItemUpdated { get; set; }

    private ModalComponent _modal = default!;
    private WishlistItemDto? _wishlistItem;

    private string _name = "";
    private string _description = "";
    private string _note = "";
    private string _price = "";

    private bool _isLoading = false;

    public async Task Open(WishlistItemDto wishlistItemDto)
    {
        _wishlistItem = wishlistItemDto;
        _name = _wishlistItem?.Name ?? "";
        _description = _wishlistItem?.Description ?? "";
        _note = _wishlistItem?.Note ?? "";
        _price = _wishlistItem?.Price ?? "";
        StateHasChanged();

        await _modal.Open();
    }

    private async Task LoadCrawledProductInformation()
    {
        if (_wishlistItem is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            var info = await ProductCrawlerService.CrawlProduct(new Uri(_wishlistItem.Url), default);
            await Task.Delay(5_000);
            _name = string.IsNullOrWhiteSpace(info.Title) ? _name : info.Title;
            _description = string.IsNullOrWhiteSpace(info.Description) ? _description : info.Description;
            _price = string.IsNullOrWhiteSpace(info.Price) && string.IsNullOrWhiteSpace(info.Currency)
                ? _price
                : $"{info.Price}{info.Currency}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SaveWishlistItem()
    {
        if (_wishlistItem is null)
        {
            return;
        }

        await ItemUpdated.InvokeAsync(_wishlistItem with
        {
            Name = _name,
            Description = _description,
            Note = _note,
            Price = _price
        });
    }

    private void OnPriorityChanged(WishlistItemPriority priority)
    {
        if (_wishlistItem is null)
        {
            return;
        }

        _wishlistItem = _wishlistItem with { Priority = priority.ToDto() };
    }
}