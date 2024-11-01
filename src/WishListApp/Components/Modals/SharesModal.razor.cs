using BlazorDialogs.Components.Modals.Base;
using BlazorDialogs.Extensions;
using BlazorDialogs.Models.Results;
using BlazorDialogs.Services;
using Microsoft.JSInterop;
using WishListApp.Models.Modals;

namespace WishListApp.Components.Modals;

public sealed partial class SharesModal(
    IJSRuntime jsRuntime,
    IMessenger messenger,
    IModalService modalService,
    IWishListShareRepository shareRepository)
    : BaseModal<SharesModalContext, None>(jsRuntime),
      IRecipient<WishListShareAdded>,
      IRecipient<WishListShareDeleted>
{
    private readonly IMessenger _messenger = messenger;
    private readonly IModalService _modalService = modalService;
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
        try
        {
            var shares = await _shareRepository.GetByWishListId(Context.WishListViewModel.WishList.Id, default);
            _shares = shares.ToModels().ToList();
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_SharesGet, exception: e);
        }
    }

    private async Task AddNewWishListShare()
    {
        try
        {
            await Context.WishListViewModel.AddWishListShare(_newShareName, default);
            _newShareName = "";
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ShareAdd, exception: e);
        }
    }
}
