using Microsoft.EntityFrameworkCore;
using WishlistApp.Components;
using WishlistApp.Data.Sql;
using WishlistApp.Identity;
using WishlistApp.ProductCrawling;
using WishlistApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddDbContext<WishlistDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("SqlServer");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();
builder.Services.AddScoped<IWishlistShareRepository, WishlistShareRepository>();
builder.Services.AddScoped<IWishlistPurchaseRepository, WishlistPurchaseRepository>();

builder.Services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMessenger, WeakReferenceMessenger>();

builder.Services.AddProductCrawler();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseIdentity();

app.Run();
