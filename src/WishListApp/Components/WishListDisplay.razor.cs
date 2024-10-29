using Microsoft.AspNetCore.Components;
using WishListApp.Models.Modals.Results;
using WishListApp.Services;

namespace WishListApp.Components;

public sealed partial class WishListDisplay(
    IModalService modalService,
    IMessenger messenger)
    : IRecipient<WishListUpdated>
{
    private readonly IModalService _modalService = modalService;
    private readonly IMessenger _messenger = messenger;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    public void Receive(WishListUpdated message)
    {
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        _messenger.RegisterAll(this);
    }

    private async Task RenameWishList()
    {
        try
        {
            var result = await _modalService.ShowTextInput(
                title: _localization.WishList_Settings_Rename_Title,
                initialText: ViewModel.WishList.Name);

            if (result.TryGetText(out var name))
            {
                await ViewModel.RenameWishList(name, default);
            }
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ListRename, exception: e);
        }
    }

    private async Task AddNewWishListItem()
    {
        try
        {
            var urlResult = await _modalService.ShowTextInput(
                title: _localization.WishList_Add_Title,
                placeholder: _localization.WishList_Add_Placeholder);

            if (!urlResult.TryGetText(out var url))
            {
                return;
            }

            var item = await ViewModel.AddWishListItem(url, default);
            if (item is null)
            {
                return;
            }

            var editResult = await _modalService.ShowWishListItemEdit(item);
            if (editResult.IsEdited())
            {
                await ViewModel.ReloadWishList(default);
            }
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemAdd, exception: e);
        }
    }

    private async Task DeletePurchasedWishListItems()
    {
        try
        {
            var result = await _modalService.ShowConfirmation(_localization.WishList_DeletePurchasedItems_Message);
            if (result.IsConfirmed())
            {
                await ViewModel.DeletePurchasedWishListItems(default);
            }
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ItemDeletePurchased, exception: e);
        }
    }

    private async Task ShowShares() => await _modalService.ShowShares(ViewModel);

    private IEnumerable<WishListItem> GetFilteredWishListItems()
    {
        if (ViewModel?.WishList is null || ViewModel.WishList.Items.Length == 0)
        {
            return [];
        }

        var filteredItems = ViewModel.HidePurchasedItems
            ? ViewModel.WishList.Items.Where(i => !i.IsPurchasedByOtherShare(ViewModel.LoggedInShare?.Id))
            : ViewModel.WishList.Items;

        filteredItems = ViewModel.HidePurchaseDetails
            ? filteredItems.OrderByDescending(i => i.Priority) // Don't order by purchase details if owner is viewing
            : filteredItems.OrderBy(
                i => !i.IsPurchased
                    ? 0 // 1st: Unpurchased items
                    : i.IsPurchasedByShare(ViewModel.LoggedInShare?.Id)
                        ? 1 // 2nd: Items purchased by currently viewing share
                        : 2) // 3rd: Items purchased by other shares
                .ThenByDescending(i => i.Priority);

        return filteredItems;
    }
}
