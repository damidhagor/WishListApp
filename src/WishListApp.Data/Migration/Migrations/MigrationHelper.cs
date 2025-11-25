using OneOf.Types;
using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations;

internal static class MigrationHelper
{
    extension(BsonDocument document)
    {
        public OneOf<Success, InvalidVersion, InvalidDocument> ValidateVersion(uint supportedVersion)
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

        public void SetVersion(uint version)
        {
            document["Version"] = version;
        }
    }

    extension(BsonValue value)
    {
        public OneOf<uint, InvalidDocument> GetVersion()
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
    }
}
