using Microsoft.AspNetCore.Components;
using WishListApp.Models.Modals.Results;
using WishListApp.Services;

namespace WishListApp.Components;

public sealed partial class ItemComponent(
    NavigationManager navigationManager,
    IModalService modalService,
    IWishListRepository repository)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IModalService _modalService = modalService;
    private readonly IWishListRepository _repository = repository;

    [CascadingParameter]
    public WishListItem Item { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private async Task OpenWishListItemEditModal()
    {
        var result = await _modalService.ShowWishListItemEdit(Item);
        if (result.IsEdited())
        {
            await ViewModel.ReloadWishList(default);
        }
    }

    private async Task MoveItem()
    {
        if (ViewModel.LoggedInUser is null)
        {
            return;
        }

        var lists = (await _repository.GetByOwnerId(ViewModel.LoggedInUser!.Identifier, default))
            .Where(l => l.Id != ViewModel.WishList.Id)
            .ToModels()
            .ToArray();

        var result = await _modalService.ShowSelectWishList(lists);
        if (!result.TryGetList(out var list))
        {
            return;
        }

        await ViewModel.MoveItemToWishList(Item, list, default);
    }

    private async Task ResetItemPurchase() => await ViewModel.ResetWishListItemPurchase(Item, default);

    private async Task SetItemPriority(int priority) => await ViewModel.SetWishListItemPriority(Item, priority, default);

    private async Task DeleteItem()
    {
        var result = await _modalService.ShowConfirmation(_localization.Item_Delete_Message);
        if (result.IsConfirmed())
        {
            await ViewModel.DeleteWishListItem(Item, default);
        }
    }
}
