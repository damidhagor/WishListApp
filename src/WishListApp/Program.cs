using WishListApp.Components;
using WishListApp.Data;
using WishListApp.Identity;
using WishListApp.ProductCrawling;
using WishListApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddWishListData(builder.Configuration);

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
