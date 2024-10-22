namespace WishListApp.Services;

public interface IMigrationService
{
    bool IsMigrationRunning { get; }

    string Status { get; }

    float MigrationProgress { get; }

    long DocumentsTotal { get; }

    long DocumentsMigrated { get; }

    void Start();

    Task Stop();
}
