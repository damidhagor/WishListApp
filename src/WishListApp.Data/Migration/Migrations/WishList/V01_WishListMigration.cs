using OneOf.Types;
using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Migrations.WishList;

internal sealed class V01_WishListMigration : IMigration<Models.WishList>
{
    public uint SupportedVersion => 0;

    public DocumentMigrationResult Migrate(BsonDocument document)
    {
        var versionResult = document.ValidateVersion(SupportedVersion);
        if (!versionResult.TryPickT0(out var success, out var errorResults))
        {
            return errorResults.Match<DocumentMigrationResult>(invalidVersion => invalidVersion, invalidDocument => invalidDocument);
        }

        var itemsResult = document.GetArrayValue("Items");
        if (!itemsResult.TryPickT0(out var items, out var remainingResults))
        {
            return remainingResults.Match<DocumentMigrationResult>(
                notFound =>
                {
                    document.SetVersion(SupportedVersion + 1);
                    return new Success();
                },
                invalidDocument => invalidDocument);
        }

        foreach (var item in items)
        {
            if (!item.IsBsonDocument)
            {
                return new InvalidDocument("Item must be a BsonDocument.");
            }

            var purchasesResult = UpdatePurchaseInformation(item.AsBsonDocument);
            if (purchasesResult.TryPickT1(out var invalidDocument, out _))
            {
                return invalidDocument;
            }

            item.AsBsonDocument.Remove("Quantity");
        }

        document.SetVersion(SupportedVersion + 1);
        return new Success();
    }

    private static OneOf<Success, InvalidDocument> UpdatePurchaseInformation(BsonDocument item)
    {
        var purchasesResult = item.GetArrayValue("Purchases");
        if (!purchasesResult.TryPickT0(out var purchases, out var remainingResults))
        {
            return remainingResults.TryPickT0(out var notFound, out var invalidDocument)
                ? new Success()
                : invalidDocument;
        }

        if (purchases.Count > 0)
        {
            var shareIdResult = purchases[0].GetObjectIdValue("ShareId");
            if (!shareIdResult.TryPickT0(out var shareId, out remainingResults))
            {
                return remainingResults.TryPickT0(out var notFound, out var invalidDocument)
                    ? new InvalidDocument("Purchase must have a field 'ShareId'.")
                    : invalidDocument;
            }

            item["Purchaser"] = shareId;
        }

        item.Remove("Purchases");
        return new Success();
    }
}
