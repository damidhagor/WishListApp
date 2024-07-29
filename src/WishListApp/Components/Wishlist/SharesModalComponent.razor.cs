using Microsoft.AspNetCore.Components;
using WishListApp.Components.Controls.Modals;

namespace WishListApp.Components.Wishlist;

public partial class SharesModalComponent
    : IRecipient<WishListShareAdded>,
      IRecipient<WishListShareDeleted>
{
    [Inject]
    private IStringLocalizer<Strings> _localizer { get; set; } = default!;

    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [Inject]
    private IWishListShareRepository _shareRepository { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ModalComponent _modal = default!;
    private List<WishListShare> _shares = [];
    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

    public async Task Open()
    {
        await _modal.Open();
        var shares = await _shareRepository.GetByWishListId(ViewModel.WishList.Id, default);
        _shares = shares.ToModels().ToList();
        StateHasChanged();
    }

    public void Receive(WishListShareAdded message)
    {
        _shares.Add(message.Share);
        StateHasChanged();
    }

    public void Receive(WishListShareDeleted message)
    {
        _shares.Remove(message.Share);
        StateHasChanged();
    }

    protected override void OnInitialized() => _messenger.RegisterAll(this);

    private async Task AddNewWishListShare()
    {
        await ViewModel.AddWishListShare(_newShareName, default);
        _newShareName = "";
    }
}
