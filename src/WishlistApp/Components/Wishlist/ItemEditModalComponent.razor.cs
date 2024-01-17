using Microsoft.AspNetCore.Components;
using WishlistApp.Components.Controls;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.Components.Wishlist;

public partial class ItemEditModalComponent
{
    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    [Inject]
    private IProductCrawlerService ProductCrawlerService { get; set; } = default!;

    private ModalComponent _modal = default!;
    private WishlistItemDto? _wishlistItem;

    private string _name = "";
    private string _description = "";
    private string _note = "";
    private string _price = "";
    private WishlistItemPriorityDto _priority = Constants.WishlistItemPriorities[0];

    private bool _isLoading = false;

    public async Task Open(WishlistItemDto wishlistItemDto)
    {
        _wishlistItem = wishlistItemDto;
        _name = _wishlistItem?.Name ?? "";
        _description = _wishlistItem?.Description ?? "";
        _note = _wishlistItem?.Note ?? "";
        _price = _wishlistItem?.Price ?? "";
        _priority = _wishlistItem?.Priority ?? Constants.WishlistItemPriorities[0];
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

        var wishlistItem = _wishlistItem with
        {
            Name = _name,
            Description = _description,
            Note = _note,
            Price = _price,
            Priority = _priority
        };

        await ViewModel.UpdateWishlistItem(wishlistItem, default);
    }
}