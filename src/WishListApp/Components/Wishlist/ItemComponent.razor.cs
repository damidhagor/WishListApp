using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Models;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.Components.Wishlist;

public partial class ItemComponent
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IProductCrawlerService _productCrawler { get; set; } = default!;

    [CascadingParameter]
    public WishListItem Item { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    protected bool IsProductInformationLoading { get; set; }

    private string? ImageUrl { get; set; }

    private bool _canBePurchased => Item.CanBePurchasedByShare(ViewModel.LoggedInShare?.Id);

    private bool _purchasedByOtherShare => Item.Purchases.Length > 0;

    private int _purchasedByLoggedInShare => Item.GetPurchasedQuantityByShare(ViewModel.LoggedInShare?.Id);

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

    private async Task BuyItem() => await ViewModel.BuyWishListItem(Item, default);

    private async Task UnbuyItem() => await ViewModel.UnbuyWishListItem(Item, default);

    private async Task SetItemPriority(WishListItemPriority priority) => await ViewModel.SetWishListItemPriority(Item, priority.Priority, default);

    private async Task DeleteItem()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie den Eintrag von der Wunschliste entfernen?");
        if (confirmed)
        {
            await ViewModel.DeleteWishListItem(Item, default);
        }
    }
}
