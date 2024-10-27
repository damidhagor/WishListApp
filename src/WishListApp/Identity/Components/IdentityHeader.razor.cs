using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WishListApp.Identity.Services;
using WishListApp.Services;

namespace WishListApp.Identity.Components;

public sealed partial class IdentityHeader(
    IIdentityService accountService,
    IMigrationService migrationService,
    [FromKeyedServices("MigrationMessenger")] IMessenger messenger)
    : IRecipient<MigrationStarted>,
      IRecipient<MigrationStopped>,
      IRecipient<MigrationProgressed>
{
    private readonly IIdentityService _accountService = accountService;
    private readonly IMigrationService _migrationService = migrationService;
    private readonly IMessenger _messenger = messenger;

    [Parameter]
    public string? Class { get; set; }

    protected override void OnInitialized() => _messenger.RegisterAll(this);

    public async void Receive(MigrationStarted message) => await InvokeAsync(StateHasChanged);

    public async void Receive(MigrationStopped message) => await InvokeAsync(StateHasChanged);

    public async void Receive(MigrationProgressed message) => await InvokeAsync(StateHasChanged);

    private static string GetUsername(AuthenticationState authenticationState)
    {
        var user = authenticationState.User;

        var nameClaim = user.Claims.FirstOrDefault(c => c.Type == "name");
        return nameClaim?.Value ?? user.Identity?.Name ?? "";
    }
}
