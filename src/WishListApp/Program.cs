using WishListApp.Components;
using WishListApp.Data;
using WishListApp.Data.Migration;
using WishListApp.Identity;
using WishListApp.ProductCrawling;
using WishListApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddServiceDefaults("wishlist-app");

builder.Services.AddLocalization();
builder.Services.AddScoped<Localization.ILocalizationService, Localization.LocalizationService>();

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddWishListData(builder.Configuration);
builder.Services.AddMigrationService();
builder.Services.AddSingleton<IMigrationService, MigrationService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMessenger, WeakReferenceMessenger>();
builder.Services.AddKeyedSingleton<IMessenger, WeakReferenceMessenger>("MigrationMessenger");

builder.Services.AddScoped<IModalService, ModalService>();

builder.Services.AddProductCrawler();

var app = builder.Build();

app.UseRequestLocalization(options =>
{
    var supportedCultures = new[] { "en-US", "de-DE" };

    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.UseIdentity();

app.Run();
