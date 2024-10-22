using WishListApp.Data.Migration.Messages;
using WishListApp.Data.Migration.Services;

namespace WishListApp.Services;

internal sealed class MigrationService
    : IMigrationService,
      IRecipient<MigrationProgress>
{
    private readonly Lock _lock = new();
    private readonly IMigrationExecutor<Data.Models.WishList> _listMigrationExecutor;
    private readonly IMigrationExecutor<Data.Models.WishListShare> _shareMigrationExecutor;
    private readonly IMessenger _messenger;

    private Task? _migrationTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsMigrationRunning { get; private set; }

    public string Status { get; private set; } = "";

    public float MigrationProgress => (float)DocumentsMigrated / DocumentsTotal * 100.0f;

    public long DocumentsTotal { get; private set; }

    public long DocumentsMigrated { get; private set; }

    public MigrationService(
        IMigrationExecutor<Data.Models.WishList> listMigrationExecutor,
        IMigrationExecutor<Data.Models.WishListShare> shareMigrationExecutor,
        [FromKeyedServices("MigrationMessenger")] IMessenger messenger)
    {
        _listMigrationExecutor = listMigrationExecutor;
        _shareMigrationExecutor = shareMigrationExecutor;
        _messenger = messenger;
        _messenger.RegisterAll(this);
    }

    public void Start()
    {
        if (IsMigrationRunning)
        {
            return;
        }

        lock (_lock)
        {
            if (IsMigrationRunning)
            {
                return;
            }

            IsMigrationRunning = true;

            _cancellationTokenSource = new();
            _migrationTask = RunMigrations(_cancellationTokenSource.Token);

            _messenger.Send(new MigrationStarted());
        }
    }

    public async Task Stop()
    {
        if (!IsMigrationRunning)
        {
            return;
        }

        try
        {
            _lock.Enter();

            if (!IsMigrationRunning)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            if (_migrationTask != null)
            {
                await _migrationTask;
            }

            IsMigrationRunning = false;
            _messenger.Send(new MigrationStopped());
        }
        finally
        {
            _lock.Exit();
        }
    }

    public void Receive(MigrationProgress message)
    {
        DocumentsTotal = message.DocumentsToMigrate;
        DocumentsMigrated = message.DocumentsMigrated;
        PublishProgress();
    }

    private async Task RunMigrations(CancellationToken cancellationToken)
    {
        try
        {
            Status = "Migrating wish lists";

            await _listMigrationExecutor.Migrate(cancellationToken);

            Status = "Migrating wish list shares";

            await _shareMigrationExecutor.Migrate(cancellationToken);

            Status = "Migration complete";
            PublishProgress();

            lock (_lock)
            {
                IsMigrationRunning = false;
                _cancellationTokenSource?.Dispose();
                _migrationTask = null;
                _messenger.Send(new MigrationStopped());
            }
        }
        catch (Exception e)
        {
            Status = $"Migration failed: {e.Message}";
        }
    }

    private void PublishProgress()
    {
        _messenger.Send(new MigrationProgressed());
    }
}
