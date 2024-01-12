using Microsoft.AspNetCore.Components;
using WishlistApp.Data.Models;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.Components.Wishlist;

public partial class ItemComponent
{
    [Inject]
    private IProductCrawlerService _productCrawler { get; set; } = default!;

    [Parameter, EditorRequired]
    public WishlistItemDto? Item { get; set; }

    [Parameter, EditorRequired]
    public WishlistShareDto? Share { get; set; }

    [Parameter, EditorRequired]
    public bool DisplayOwnerControls { get; set; }

    [Parameter, EditorRequired]
    public bool DisplayBuyInformation { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemBought { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemUnbought { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemDeleted { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemUpdated { get; set; }

    [Parameter]
    public EventCallback<WishlistItemPriorityDto> PriorityChanged { get; set; }

    protected bool IsProductInformationLoading { get; set; }

    protected string? Name { get; set; }

    private string? Description { get; set; }

    private string? Note { get; set; }

    private string? ImageUrl { get; set; }

    private string? Price { get; set; }

    private ItemEditModalComponent _itemEditModal = default!;

    private readonly WishlistItemPriorityDto[] _priorities =
        [
            WishlistItemPriority.Unknown.ToDto(),
            WishlistItemPriority.Low.ToDto(),
            WishlistItemPriority.Medium.ToDto(),
            WishlistItemPriority.High.ToDto(),
            WishlistItemPriority.VeryHigh.ToDto()
        ];

    protected override async Task OnParametersSetAsync()
    {
        await LoadItemInformation(default);
    }

    private async Task OpenWishlistItemEditModal(WishlistItemDto wishlistItemDto)
    {
        await _itemEditModal.Open(wishlistItemDto);
    }

    private async Task LoadItemInformation(CancellationToken cancellationToken)
    {
        return;
        if (Item is null)
        {
            return;
        }

        Name = string.IsNullOrWhiteSpace(Item.Name) ? Item.Url : Item.Name;
        Description = Item.Description;
        Note = Item.Note;
        Price = Item.Price;

        try
        {
            IsProductInformationLoading = true;
            var result = await _productCrawler.CrawlProduct(new Uri(Item.Url), cancellationToken);

            Name = string.IsNullOrWhiteSpace(result.Title) ? Item.Url : result.Title;
            Description = string.IsNullOrWhiteSpace(result.Description) ? Item.Description : result.Description;
            ImageUrl = result.ImageUrl;
            Price = string.IsNullOrWhiteSpace(result.Price) ? Item.Price : $"{result.Price}{result.Currency}";
        }
        catch (Exception e)
        {
            ;
        }
        finally
        {
            IsProductInformationLoading = false;
        }
    }
}