using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WishlistApp.Data;

namespace WishlistApp.Authentication;

internal sealed class WishlistShareAuthenticationSchemeHandler(
    IOptionsMonitor<WishlistShareAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    WishlistDbContext dbContext)
    : AuthenticationHandler<WishlistShareAuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "WishlistShare";

    private readonly WishlistDbContext _dbContext = dbContext;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!string.IsNullOrWhiteSpace(Options.AccessKeyRouteValueKey)
            && Context.Request.RouteValues.TryGetValue("accessKey", out var accessKeyValue)
            && accessKeyValue is string accessKey)
        {
            var wishlistShare = await _dbContext.WishlistShares.FirstOrDefaultAsync(s => s.AccessKey == accessKey, Context.RequestAborted);

            if (wishlistShare is not null)
            {
                var claims = new[] { new Claim("AccessKey", accessKey) };

                var identity = new ClaimsIdentity(claims, SchemeName);

                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, SchemeName);

                return AuthenticateResult.Success(ticket);
            }
        }

        return AuthenticateResult.NoResult();
    }
}