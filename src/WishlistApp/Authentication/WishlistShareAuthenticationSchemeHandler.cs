using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using WishlistApp.Services;
using Constants = WishlistApp.Authentication.WishlistShareAuthenticationConstants;

namespace WishlistApp.Authentication;

internal sealed class WishlistShareAuthenticationSchemeHandler(
    IOptionsMonitor<WishlistShareAuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IWishlistRepository repository)
    : AuthenticationHandler<WishlistShareAuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly IWishlistRepository _repository = repository;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (Context.Request.RouteValues.TryGetValue(Constants.AccessKeyRouteValueKey, out var accessKeyValue)
                && accessKeyValue is string accessKey)
            {
                var wishlistShare = await _repository.GetWishlistShareByAccessKey(accessKey, Context.RequestAborted);

                if (wishlistShare is not null)
                {
                    var ticket = CreateAuthenticationTicket(wishlistShare);
                    return AuthenticateResult.Success(ticket);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Authentication failed.");
            return AuthenticateResult.Fail(e);
        }

        return AuthenticateResult.NoResult();
    }

    private AuthenticationTicket CreateAuthenticationTicket(WishlistShare wishlistShare)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, wishlistShare.Name),
            new Claim(ClaimTypes.Role, Constants.WishlistShareRole),
            new Claim(Constants.WishlistShareIdClaim, wishlistShare.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, Constants.AuthenticationSchemeName);

        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationTicket(principal, Constants.AuthenticationSchemeName);
    }
}