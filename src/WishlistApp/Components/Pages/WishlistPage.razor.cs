using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistPage
{
    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private WishlistShareDto? _wishlistShare;

    private string? _userIdentifier;

    private WishlistDto? _wishlist;


    protected override async Task OnInitializedAsync()
    {
        await LoadWishlistShare(default);
        await LoadUserIdentifier(default);

        await LoadWishlistAndValidateAccess(default);
    }

    private async Task LoadWishlistShare(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(AccessKey))
        {
            _wishlistShare = await Repository.GetWishlistShareByAccessKey(AccessKey, cancellationToken);
        }
    }

    private async Task LoadUserIdentifier(CancellationToken cancellationToken)
    {
        if (AuthenticationStateTask is null)
        {
            return;
        }

        var authenticationState = await AuthenticationStateTask;
        _userIdentifier = authenticationState.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;
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

        var validOwner = !string.IsNullOrWhiteSpace(_userIdentifier)
            && wishlist.OwnerIdentifier == _userIdentifier;

        var validShare = _wishlistShare is not null
            && wishlist.Id == _wishlistShare.WishlistId;

        if (!validOwner && !validShare)
        {
            NavigationManager.NavigateTo("/");
        }

        _wishlist = wishlist;
    }
}