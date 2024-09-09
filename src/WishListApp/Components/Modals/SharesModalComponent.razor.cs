using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Modals;

public partial class SharesModalComponent(
    IMessenger messenger,
    IWishListShareRepository shareRepository)
    : IRecipient<WishListShareAdded>,
      IRecipient<WishListShareDeleted>
{
    private readonly IMessenger _messenger = messenger;
    private readonly IWishListShareRepository _shareRepository = shareRepository;

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
