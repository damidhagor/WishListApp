using System.Runtime.CompilerServices;
using OneOf.Types;
using WishListApp.Data.Migration.Results;
using WishListApp.Data.Migration.Services;

namespace WishListApp.Data.Accessors;

internal static class MigrationExecutorAccessors<T>
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ValidateMigrationVersions")]
    public extern static OneOf<Success, InvalidMigrationVersion> ValidateMigrationVersions(MigrationExecutor<T> executor);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MigrateBatch")]
    public extern static OneOf<List<ReplaceOneModel<BsonDocument>>, InvalidDocument> MigrateBatch(MigrationExecutor<T> executor, BsonDocument[] documents);
}
