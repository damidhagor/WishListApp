using Microsoft.AspNetCore.Components;
using WishListApp.Components.Controls.Modals;

namespace WishListApp.Components.Wishlist;

public partial class SharesModalComponent : IRecipient<WishListUpdated>
{
    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ModalComponent _modal = default!;
    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

    public async Task Open() => await _modal.Open();

    public void Receive(WishListUpdated message) => StateHasChanged();

    protected override void OnInitialized() => _messenger.RegisterAll(this);

    private async Task AddNewWishlistShare()
    {
        await ViewModel.AddWishlistShare(_newShareName, default);
        _newShareName = "";
    }
}