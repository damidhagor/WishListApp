namespace WishListApp.Data.Migration.Messages;

public sealed record MigrationProgress(bool IsRunning, long DocumentsToMigrate, long DocumentsMigrated);
