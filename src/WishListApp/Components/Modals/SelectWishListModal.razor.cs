using Microsoft.JSInterop;
using Shared.Blazor.Dialogs.Components.Modals.Base;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components.Modals;

public sealed partial class SelectWishListModal(IJSRuntime jsRuntime)
    : BaseModal<SelectWishListModalContext, SelectWishListResult>(jsRuntime)
{
    private WishList? _selectedList;

    private bool _isConfirmButtonDisabled => _selectedList is null;

    protected override void OnParametersSet()
    {
        _selectedList = Context.Lists.FirstOrDefault();
        base.OnParametersSet();
    }

    private async Task OnConfirm()
    {
        if (_selectedList is null)
        {
            return;
        }

        await Close(new Selected(_selectedList));
    }
}
