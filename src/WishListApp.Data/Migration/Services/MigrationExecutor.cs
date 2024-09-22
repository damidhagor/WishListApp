using CommunityToolkit.Mvvm.Messaging;
using OneOf.Types;
using WishListApp.Data.Migration.Messages;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration.Services;

internal sealed class MigrationExecutor<T>(
    IMongoCollection<BsonDocument> collection,
    IEnumerable<IMigration<T>> migrations,
    IMessenger messenger)
    : IMigrationExecutor<T>
{
    private IMessenger _messenger = messenger;
    private readonly IMongoCollection<BsonDocument> _collection = collection;
    private readonly IMigration<T>[] _migrations = [.. migrations.OrderBy(m => m.SupportedVersion)];

    public MigrationStatus Status { get; private set; } = new(false, null, 0, 0);

    public async Task<MigrationResult> Migrate(CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = ValidateMigrationVersions();
            if (validationResult.TryPickT1(out var invalidVersions, out _))
            {
                return new MigrationError($"Migration versions are invalid. Version {invalidVersions.ExpectedVersion} is expected but actual version is {invalidVersions.SupportedVersion}.");
            }

            var totalDocuments = await _collection.CountDocumentsAsync(Builders<BsonDocument>.Filter.Empty, cancellationToken: cancellationToken);

            using var cursor = await _collection.FindAsync(
                Builders<BsonDocument>.Filter.Empty,
                new FindOptions<BsonDocument>
                {
                    BatchSize = 100,
                    NoCursorTimeout = true
                },
                cancellationToken);

            var documentsMigrated = 0L;
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                var documents = cursor.Current.ToArray();
                foreach (var document in documents)
                {
                    var batchResult = MigrateBatch(documents, cancellationToken);
                    if (!batchResult.TryPickT0(out var replaceOperations, out var invalidDocument))
                    {
                        return new MigrationError($"The migration encountered an invalid document: {invalidDocument.Error}");
                    }

                    await _collection.BulkWriteAsync(replaceOperations, cancellationToken: cancellationToken);

                    documentsMigrated += replaceOperations.Count;
                    UpdateProgress(totalDocuments, documentsMigrated);
                }
            }

            return new Success();
        }
        catch (Exception ex)
        {
            return new MigrationError($"The migration of the WishListShares collection failed: {ex.Message}");
        }
    }

    private OneOf<Success, InvalidMigrationVersion> ValidateMigrationVersions()
    {
        for (var i = 0u; i < _migrations.Length; i++)
        {
            if (_migrations[i].SupportedVersion != i)
            {
                return new InvalidMigrationVersion(i, _migrations[i].SupportedVersion);
            }
        }

        return new Success();
    }

    private OneOf<List<ReplaceOneModel<BsonDocument>>, InvalidDocument> MigrateBatch(BsonDocument[] documents, CancellationToken cancellationToken)
    {
        var replaceOperations = new List<ReplaceOneModel<BsonDocument>>(documents.Length);
        foreach (var document in documents)
        {
            var versionResult = document.GetVersion();
            if (!versionResult.TryPickT0(out var version, out var invalidDocument))
            {
                return invalidDocument;
            }

            var objectIdResult = document.GetObjectIdValue("_id");
            if (!objectIdResult.TryPickT0(out var objectId, out var idErrorResults))
            {
                return idErrorResults.Match(
                    elementNotFound => new InvalidDocument("The document is missing an '_id' field."),
                    invalidDocument => invalidDocument);
            }

            if (version >= _migrations.Length)
            {
                continue;
            }

            for (var i = version; i < _migrations.Length; i++)
            {
                var migrationResult = _migrations[i].Migrate(document);
                if (!migrationResult.TryPickT0(out var migrated, out var migrationErrorResults))
                {
                    return migrationErrorResults.Match(
                        invalidVersion => new InvalidDocument($"The document's version {invalidVersion.Version} does not match the supported version {invalidVersion.SupportedVersion}."),
                        invalidDocument => invalidDocument);
                }
            }

            replaceOperations.Add(new ReplaceOneModel<BsonDocument>(
                Builders<BsonDocument>.Filter.Eq("_id", objectId),
                document));
        }

        return replaceOperations;
    }

    private void UpdateProgress(long totalDocuments, long documentsMigrated)
    {
        Status = new(true, null, totalDocuments, documentsMigrated);
        _messenger.Send(new MigrationProgress(totalDocuments, documentsMigrated));
    }
}
