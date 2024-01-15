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

    private string? ImageUrl { get; set; }

    private ItemEditModalComponent _itemEditModal = default!;

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
        if (Item is null)
        {
            return;
        }

        try
        {
            IsProductInformationLoading = true;
            var result = await _productCrawler.CrawlProduct(new Uri(Item.Url), cancellationToken);

            ImageUrl = result.ImageUrl;
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