using Microsoft.AspNetCore.Components;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.Components;

public partial class ItemEditComponent(
    ILocalizationService_Localization localizer,
    IProductCrawlerService productCrawlerService,
    IWishListItemRepository itemRepository)
{
    private readonly ILocalizationService_Localization _localizer = localizer;
    private readonly IProductCrawlerService _productCrawlerService = productCrawlerService;
    private readonly IWishListItemRepository _itemRepository = itemRepository;

    private string _imageUrl = "";
    private string _siteName = "";
    private string _name = "";
    private string _description = "";
    private string _note = "";
    private decimal _price = 0;
    private string _currency = "";
    private int _priority = WishListItem.AvailablePriorities[0];

    [Parameter]
    public WishListItem? Item { get; set; }

    protected override void OnParametersSet()
    {
        _imageUrl = Item?.ImageUrl ?? "";
        _siteName = Item?.SiteName ?? "";
        _name = Item?.Name ?? "";
        _description = Item?.Description ?? "";
        _note = Item?.Note ?? "";
        _price = Item?.Price ?? 0;
        _currency = Item?.Currency ?? "";
        _priority = Item?.Priority ?? WishListItem.AvailablePriorities[0];
    }

    public async Task LoadProductInformation()
    {
        if (Item is null)
        {
            return;
        }

        var info = await _productCrawlerService.CrawlProduct(new Uri(Item.Url), default);
        _imageUrl = string.IsNullOrWhiteSpace(info.ImageUrl) ? _imageUrl : info.ImageUrl;
        _siteName = string.IsNullOrWhiteSpace(info.SiteName) ? _siteName : info.SiteName;
        _name = string.IsNullOrWhiteSpace(info.Title) ? _name : info.Title;
        _description = string.IsNullOrWhiteSpace(info.Description) ? _description : info.Description;
        _price = info.Price ?? _price;
        _currency = string.IsNullOrWhiteSpace(info.Currency) ? _currency : info.Currency;

        StateHasChanged();
    }

    public async Task SaveWishListItem()
    {
        if (Item is null)
        {
            return;
        }

        var updatedItem = Item with
        {
            ImageUrl = _imageUrl,
            SiteName = _siteName,
            Name = _name,
            Description = _description,
            Note = _note,
            Price = _price,
            Currency = _currency,
            Priority = _priority
        };

        await _itemRepository.Update(updatedItem.WishListId, updatedItem.ToDataModel(), default);
    }
}
