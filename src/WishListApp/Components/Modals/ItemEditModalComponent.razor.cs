using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals;

public partial class ItemEditModalComponent(IStringLocalizer<Localization> localizer)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;

    private ModalComponent _modal = default!;
    private ItemEditComponent _itemEditComponent = default!;
    private WishListItem? _item;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    public async Task Open(WishListItem item)
    {
        _item = item;
        StateHasChanged();

        await _modal.Open();
    }

    private async Task LoadProductInformation() => await _itemEditComponent.LoadProductInformation();

    private async Task SaveWishListItem()
    {
        await _itemEditComponent.SaveWishListItem();
        await ViewModel.ReloadWishList(default);
    }
}
