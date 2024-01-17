using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.Components.Wishlist;

public partial class ItemComponent
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IProductCrawlerService _productCrawler { get; set; } = default!;

    [CascadingParameter]
    public WishlistItemDto Item { get; set; } = default!;

    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    protected bool IsProductInformationLoading { get; set; }

    private string? ImageUrl { get; set; }

    private bool _itemCanBeBought => ViewModel.LoggedInShare is not null && Item?.BuyerShareId is null;

    private bool _itemIsBoughtByOtherShare => (ViewModel.LoggedInShare is not null && Item?.BuyerShareId != ViewModel.LoggedInShare.Id)
                                           || (ViewModel.LoggedInShare is null && Item?.BuyerShareId is not null);

    private bool _itemIsBoughtByLoggedInShare => ViewModel.LoggedInShare is not null && Item?.BuyerShareId == ViewModel.LoggedInShare.Id;

    private ItemEditModalComponent _itemEditModal = default!;

    protected override async Task OnParametersSetAsync()
    {
        await LoadItemInformation(default);
    }

    private async Task OpenWishlistItemEditModal() => await _itemEditModal.Open(Item);

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

    private async Task BuyItem() => await ViewModel.BuyWishlistItem(Item, default);

    private async Task UnbuyItem() => await ViewModel.UnbuyWishlistItem(Item, default);

    private async Task SetItemPriority(WishlistItemPriorityDto priority) => await ViewModel.SetWishlistItemPriority(Item, priority, default);

    private async Task DeleteItem()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie den Eintrag von der Wunschliste entfernen?");
        if (confirmed)
        {
            await ViewModel.DeleteWishlistItem(Item, default);
        }
    }
}