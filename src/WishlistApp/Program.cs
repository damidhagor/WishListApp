using Microsoft.EntityFrameworkCore;
using WishlistApp.Authentication;
using WishlistApp.Components;
using WishlistApp.Data;
using WishlistApp.Extensions;
using WishlistApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(WishlistShareAuthenticationSchemeHandler.SchemeName)
    .AddScheme<WishlistShareAuthenticationSchemeOptions, WishlistShareAuthenticationSchemeHandler>(
        WishlistShareAuthenticationSchemeHandler.SchemeName,
        options => options.AccessKeyRouteValueKey = "accessKey");

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy(
        "WishlistSharePolicy",
        policy => policy
            .RequireClaim("AccessKey")
            .AddAuthenticationSchemes(WishlistShareAuthenticationSchemeHandler.SchemeName));
});

builder.Services.AddWishlistIdentity();

builder.Services.AddDbContext<WishlistDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("SqlServer");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

await app.SeedAdminUserAndRole();

app.Run();
