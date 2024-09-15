using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations.WishListShare;

internal sealed class V01_WishListShareMigration : IMigration<Models.WishListShare>
{
    public uint SourceVersion => 0;

    public uint TargetVersion => 1;

    public MigrationResult Migrate(BsonDocument document)
    {
        var versionResult = document.ValidateVersion(SourceVersion);
        if (!versionResult.TryPickT0(out var success, out var errorResults))
        {
            return errorResults.Match<MigrationResult>(invalidVersion => invalidVersion, invalidDocument => invalidDocument);
        }

        document.SetVersion(TargetVersion);
        return new Migrated(SourceVersion, TargetVersion);
    }
}
