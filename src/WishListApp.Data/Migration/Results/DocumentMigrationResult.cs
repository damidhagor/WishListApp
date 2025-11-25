using OneOf.Types;

namespace WishListApp.Data.Migration.Results;

[GenerateOneOf]
public sealed partial class DocumentMigrationResult : OneOfBase<Success, InvalidVersion, InvalidDocument>;
