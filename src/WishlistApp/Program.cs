using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Components;
using WishlistApp.Data;
using WishlistApp.Data.Models;
using WishlistApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<WishlistDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("SqlServer");
    options.UseNpgsql(connectionString);
});

builder.Services.AddIdentity<WishlistUser, IdentityRole>()
    .AddEntityFrameworkStores<WishlistDbContext>();

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
