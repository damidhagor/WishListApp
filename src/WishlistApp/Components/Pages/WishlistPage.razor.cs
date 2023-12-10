using Microsoft.AspNetCore.Components;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistPage
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private WishlistShareDto? _wishlistShare;

    private WishlistUserDto? _user;

    private WishlistDto? _wishlist;


    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(AccessKey))
        {
            _wishlistShare = await Repository.GetWishlistShareByAccessKey(AccessKey, default);
        }

        _user = await UserService.GetLoggedInWishlistUser();

        await LoadWishlistAndValidateAccess(default);
    }

    private async Task LoadWishlistAndValidateAccess(CancellationToken cancellationToken)
    {
        var wishlistId = _wishlistShare?.WishlistId ?? WishlistId;

        var wishlist = wishlistId is not null
            ? await Repository.GetWishlist(wishlistId.Value, cancellationToken)
            : null;

        if (wishlist is null)
        {
            NavigationManager.NavigateTo("/not-found");
            return;
        }

        var validOwner = _user is not null
            && wishlist.OwnerIdentifier == _user.Identifier;

        var validShare = _wishlistShare is not null
            && wishlist.Id == _wishlistShare.WishlistId;

        if (!validOwner && !validShare)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        _wishlist = wishlist;
    }
}