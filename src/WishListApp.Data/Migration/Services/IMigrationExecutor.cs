using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Services;

public interface IMigrationExecutor<T>
{
    public MigrationStatus Status { get; }

    Task<MigrationResult> Migrate(CancellationToken cancellationToken);
}
