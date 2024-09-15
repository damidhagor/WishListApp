using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations;

internal interface IMigration<T>
{
    uint SourceVersion { get; }

    uint TargetVersion { get; }

    MigrationResult Migrate(BsonDocument document);
}
