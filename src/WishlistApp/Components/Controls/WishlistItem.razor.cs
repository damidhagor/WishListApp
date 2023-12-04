using Microsoft.AspNetCore.Components;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistItem
{
    [Inject]
    private IProductCrawlerService _productCrawler { get; set; } = default!;

    [Parameter]
    public WishlistItemDto? Item { get; set; }

    [Parameter]
    public WishlistShareDto? Share { get; set; }

    [Parameter]
    public bool IsEditable { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemBought { get; set; }

    [Parameter]
    public EventCallback<WishlistItemDto> ItemUnbought { get; set; }

    protected bool IsProductInformationLoading { get; set; }

    protected string? Name { get; set; }

    private string? Description { get; set; }

    private string? ImageUrl { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadItemInformation(default);
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

            Name = result.Title ?? Item.Url;
            Description = result.Description;
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