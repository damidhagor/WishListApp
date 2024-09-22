using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations;

internal interface IMigration<T>
{
    uint SupportedVersion { get; }

    DocumentMigrationResult Migrate(BsonDocument document);
}
