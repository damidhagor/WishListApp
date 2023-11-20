using Microsoft.AspNetCore.Identity;

namespace WishlistApp.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task<WebApplication?> SeedAdminUserAndRole(this WebApplication? app)
    {
        if (app is null)
        {
            return app;
        }

        using var scope = app.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<WishlistRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<WishlistUser>>();

        var username = app.Configuration.GetValue<string>("AdminUser:Username") ?? throw new ArgumentNullException("username", "Admin user name is needed");
        var password = app.Configuration.GetValue<string>("AdminUser:Password") ?? throw new ArgumentNullException("password", "Admin password is needed");

        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole is null)
        {
            adminRole = new() { Name = "Admin", };
            await roleManager.CreateAsync(adminRole);
        }

        var adminUser = await userManager.FindByNameAsync(username);
        if (adminUser is null)
        {
            adminUser = new()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            await userManager.CreateAsync(adminUser);
            await userManager.AddPasswordAsync(adminUser, password);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        return app;
    }
}
