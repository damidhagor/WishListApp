namespace WishListApp.Data.Migration.Results;

public sealed record InvalidMigrationVersion(uint ExpectedVersion, uint SupportedVersion);
