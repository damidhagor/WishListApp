using OneOf.Types;
using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations;

internal static class MigrationHelper
{
    public static OneOf<Success, InvalidVersion, InvalidDocument> ValidateVersion(this BsonDocument document, uint supportedVersion)
    {
        var versionResult = document.GetVersion();
        if (!versionResult.TryPickT0(out var version, out var invalidDocument))
        {
            return invalidDocument;
        }

        return version != supportedVersion
            ? new InvalidVersion(supportedVersion, version)
            : new Success();
    }

    public static OneOf<uint, InvalidDocument> GetVersion(this BsonValue value)
    {
        var versionResult = value.GetInt64Value("Version");

        if (!versionResult.TryPickT0(out var versionValue, out var errorResults))
        {
            if (errorResults.TryPickT1(out var invalidDocument, out var notFound))
            {
                return invalidDocument;
            }

            return 0u;
        }

        return versionValue >= 0
            ? (uint)versionValue
            : new InvalidDocument("'Version' field must be a positive integer.");
    }

    public static void SetVersion(this BsonDocument document, uint version)
    {
        document["Version"] = version;
    }
}
