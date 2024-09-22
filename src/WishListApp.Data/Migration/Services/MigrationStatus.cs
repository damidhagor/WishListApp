namespace WishListApp.Data.Migration.Services;

public sealed record MigrationStatus(bool IsRunning, string? Error, long DocumentsToMigrate, long DocumentsMigrated);
