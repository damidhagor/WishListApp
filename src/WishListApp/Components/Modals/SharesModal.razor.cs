using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;

namespace WishListApp.Components.Modals;

public sealed partial class SharesModal(
    IJSRuntime jsRuntime,
    IMessenger messenger,
    IWishListShareRepository shareRepository)
    : BaseModal<SharesModalContext, None>(jsRuntime),
      IRecipient<WishListShareAdded>,
      IRecipient<WishListShareDeleted>
{
    private readonly IMessenger _messenger = messenger;
    private readonly IWishListShareRepository _shareRepository = shareRepository;

    private List<WishListShare> _shares = [];
    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _messenger.RegisterAll(this);
    }

    protected override async Task OnParametersSetAsync()
    {
        var shares = await _shareRepository.GetByWishListId(Context.WishListViewModel.WishList.Id, default);
        _shares = shares.ToModels().ToList();
    }

    private async Task AddNewWishListShare()
    {
        await Context.WishListViewModel.AddWishListShare(_newShareName, default);
        _newShareName = "";
    }
}
