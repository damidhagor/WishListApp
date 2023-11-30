using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WishlistApp.Data.Models;

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
        var expectedRoles = app.Configuration.GetSection("AdminUser:Roles").Get<string[]>() ?? [];
        var expectedClaims = app.Configuration.GetSection("AdminUser:Claims").Get<Dictionary<string, string>>() ?? [];

        foreach (var expectedRole in expectedRoles)
        {
            var role = await roleManager.FindByNameAsync(expectedRole);
            if (role is null)
            {
                role = new() { Name = expectedRole };
                await roleManager.CreateAsync(role);
            }
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
        }

        var existingRoles = await userManager.GetRolesAsync(adminUser);
        await userManager.RemoveFromRolesAsync(adminUser, existingRoles);
        await userManager.AddToRolesAsync(adminUser, expectedRoles);

        var existingClaims = await userManager.GetClaimsAsync(adminUser);
        await userManager.RemoveClaimsAsync(adminUser, existingClaims);
        await userManager.AddClaimsAsync(adminUser, expectedClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));

        return app;
    }
}
