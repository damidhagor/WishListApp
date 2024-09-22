using OneOf.Types;
using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations.WishListShare;

internal sealed class V01_WishListShareMigration : IMigration<Models.WishListShare>
{
    public uint SupportedVersion => 0;

    public DocumentMigrationResult Migrate(BsonDocument document)
    {
        var versionResult = document.ValidateVersion(SupportedVersion);
        if (!versionResult.TryPickT0(out var success, out var errorResults))
        {
            return errorResults.Match<DocumentMigrationResult>(invalidVersion => invalidVersion, invalidDocument => invalidDocument);
        }

        document.SetVersion(SupportedVersion + 1);
        return new Success();
    }
}
