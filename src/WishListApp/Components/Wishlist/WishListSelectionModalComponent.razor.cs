using Microsoft.AspNetCore.Components;
using WishListApp.Components.Controls.Modals;

namespace WishListApp.Components.Wishlist;

public partial class WishListSelectionModalComponent
{
    [Inject]
    private IStringLocalizer<Localization> _localizer { get; set; } = default!;

    [Inject]
    private IWishListRepository _repository { get; set; } = default!;

    private ModalComponent _modal = default!;
    private WishListItem? _item;
    private WishList[] _lists = [];
    private WishList? _selectedList;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    public async Task Open(WishListItem item)
    {
        _item = item;

        _lists = [];
        if (ViewModel.LoggedInUser is not null)
        {
            _lists = (await _repository.GetByOwner(ViewModel.LoggedInUser.Identifier, default))
                .Where(l => l.Id != ViewModel.WishList.Id)
                .ToModels()
                .ToArray();
            _selectedList = _lists.FirstOrDefault();
        }

        StateHasChanged();

        await _modal.Open();
    }

    private async Task MoveItemToWishList()
    {
        if (_item is null || _selectedList is null)
        {
            return;
        }

        await _modal.Close();
        await ViewModel.MoveItemToWishList(_item, _selectedList, default);
    }
}
