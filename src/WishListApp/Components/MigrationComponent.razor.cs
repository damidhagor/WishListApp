using Microsoft.AspNetCore.Components;
using WishListApp.Services;

namespace WishListApp.Components;

public partial class MigrationComponent(
    [FromKeyedServices("MigrationMessenger")] IMessenger messenger,
    IMigrationService migrationService)
    : IRecipient<MigrationStarted>,
      IRecipient<MigrationStopped>,
      IRecipient<MigrationProgressed>
{
    private readonly IMessenger _messenger = messenger;
    private readonly IMigrationService _migrationService = migrationService;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized() => _messenger.RegisterAll(this);

    public async void Receive(MigrationStopped message)
    {
        await InvokeAsync(StateHasChanged);
    }

    public async void Receive(MigrationStarted message)
    {
        await InvokeAsync(StateHasChanged);
    }

    public async void Receive(MigrationProgressed message)
    {
        await InvokeAsync(StateHasChanged);
    }
}
