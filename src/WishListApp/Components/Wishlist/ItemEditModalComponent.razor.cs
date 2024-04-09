using Microsoft.AspNetCore.Components;
using WishListApp.Components.Controls.Modals;
using WishListApp.Models;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.Components.Wishlist;

public partial class ItemEditModalComponent
{
    [Inject]
    private IProductCrawlerService _productCrawlerService { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ModalComponent _modal = default!;
    private WishListItem? _item;

    private string _name = "";
    private string _description = "";
    private string _note = "";
    private decimal _price = 0;
    private string _currency = "";
    private int _quantity = 1;
    private WishListItemPriority _priority = Constants.WishListItemPriorities[0];

    private bool _isLoading = false;

    public async Task Open(WishListItem item)
    {
        _item = item;
        _name = _item?.Name ?? "";
        _description = _item?.Description ?? "";
        _note = _item?.Note ?? "";
        _price = _item?.Price ?? 0;
        _currency = _item?.Currency ?? "";
        _quantity = _item?.Quantity ?? 1;
        _priority = _item?.Priority ?? Constants.WishListItemPriorities[0];
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
            _price = info.Price ?? _price;
            _currency = string.IsNullOrWhiteSpace(info.Currency) ? _currency : info.Currency;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SaveWishListItem()
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
            Currency = _currency,
            Quantity = _quantity,
            Priority = _priority
        };

        await ViewModel.UpdateWishListItem(updatedItem, default);
    }
}
