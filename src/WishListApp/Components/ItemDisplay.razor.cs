using BlazorDialogs.Extensions;
using BlazorDialogs.Services;
using Microsoft.AspNetCore.Components;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components;

public sealed partial class ItemDisplay(
    IModalService modalService,
    IWishListRepository repository)
{
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
            try
            {
                await ViewModel.ReloadWishList(default);
            }
            catch (Exception e)
            {
                await _modalService.ShowError(_localization.Error_ListReload, exception: e);
            }
        }
    }

    private async Task MoveItem()
    {
        if (ViewModel.LoggedInUser is null)
        {
            return;
        }

        try
        {
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
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemMove, exception: e);
        }
    }

    private async Task ResetItemPurchase()
    {
        try
        {
            await ViewModel.ResetWishListItemPurchase(Item, default);
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemPurchaseReset, exception: e);
        }
    }

    private async Task SetItemPriority(int priority)
    {
        try
        {
            await ViewModel.SetWishListItemPriority(Item, priority, default);
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemPrioritySet, exception: e);
        }
    }

    private async Task DeleteItem()
    {
        try
        {
            var result = await _modalService.ShowConfirmation(_localization.Item_Delete_Message);
            if (result.IsConfirmed())
            {
                await ViewModel.DeleteWishListItem(Item, default);
            }
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemDelete, exception: e);
        }
    }
}
