using Microsoft.AspNetCore.Components;
using WishListApp.Components.Controls.Modals;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.Components.Wishlist;

public partial class ItemEditModalComponent
{
    [Inject]
    private IProductCrawlerService _productCrawlerService { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ModalComponent _modal = default!;
    private WishlistItem? _item;

    private string _name = "";
    private string _description = "";
    private string _note = "";
    private string _price = "";
    private int _quantity = 1;
    private WishlistItemPriority _priority = Constants.WishlistItemPriorities[0];

    private bool _isLoading = false;

    public async Task Open(WishlistItem item)
    {
        _item = item;
        _name = _item?.Name ?? "";
        _description = _item?.Description ?? "";
        _note = _item?.Note ?? "";
        _price = _item?.Price ?? "";
        _quantity = _item?.Quantity ?? 1;
        _priority = _item?.Priority ?? Constants.WishlistItemPriorities[0];
        StateHasChanged();

        await _modal.Open();
    }

    private async Task LoadCrawledProductInformation()
    {
        if (_item is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            var info = await _productCrawlerService.CrawlProduct(new Uri(_item.Url), default);
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
        if (_item is null)
        {
            return;
        }

        var updatedItem = _item with
        {
            Name = _name,
            Description = _description,
            Note = _note,
            Price = _price,
            Quantity = _quantity,
            Priority = _priority
        };

        await ViewModel.UpdateWishlistItem(updatedItem, default);
    }
}