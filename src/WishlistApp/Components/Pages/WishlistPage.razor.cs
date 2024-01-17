using Microsoft.AspNetCore.Components;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistPage : IRecipient<WishlistUpdated>
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private IWishlistRepository WishlistRepository { get; set; } = default!;

    [Inject]
    private IWishlistItemRepository ItemRepository { get; set; } = default!;

    [Inject]
    private IWishlistShareRepository ShareRepository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IMessenger Messenger { get; set; } = default!;

    private WishlistViewModel? _viewModel;


    protected override async Task OnInitializedAsync()
    {
        Messenger.RegisterAll(this);

        var share = !string.IsNullOrWhiteSpace(AccessKey)
            ? await ShareRepository.GetWishlistShareByAccessKey(AccessKey, default)
            : null;

        var user = await UserService.GetLoggedInWishlistUser();

        await LoadWishlistAndValidateAccess(user, share, default);
    }

    private async Task LoadWishlistAndValidateAccess(WishlistUserDto? user, WishlistShareDto? share, CancellationToken cancellationToken)
    {
        var wishlistId = share?.WishlistId ?? WishlistId;

        var wishlist = wishlistId is not null
            ? await WishlistRepository.GetWishlist(wishlistId.Value, cancellationToken)
            : null;

        if (wishlist is null)
        {
            NavigationManager.NavigateTo("/not-found");
            return;
        }

        var validOwner = user is not null
            && wishlist.OwnerIdentifier == user.Identifier;

        var validShare = share is not null
            && wishlist.Id == share.WishlistId;

        if (!validOwner && !validShare)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        _viewModel = new WishlistViewModel(WishlistRepository, ItemRepository, ShareRepository, NavigationManager, Messenger, wishlist, user, share);
    }

    public void Receive(WishlistUpdated message) => StateHasChanged();
}