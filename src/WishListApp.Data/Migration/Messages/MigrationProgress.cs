namespace WishListApp.Data.Migration.Messages;

public sealed record MigrationProgress(long DocumentsToMigrate, long DocumentsMigrated);
