using Microsoft.AspNetCore.Components;
using WishlistApp.Components.Controls;

namespace WishlistApp.Components.Wishlist;

public partial class SharesModalComponent : IRecipient<WishlistUpdated>
{
    [Inject]
    private IMessenger Messenger { get; set; } = default!;

    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    private ModalComponent _modal = default!;
    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

    public async Task Open() => await _modal.Open();

    public void Receive(WishlistUpdated message) => StateHasChanged();

    protected override void OnInitialized() => Messenger.RegisterAll(this);

    private async Task AddNewWishlistShare()
    {
        await ViewModel.AddWishlistShare(_newShareName, default);
        _newShareName = "";
    }
}