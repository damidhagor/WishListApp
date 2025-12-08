using OneOf.Types;

namespace WishListApp.Data.Migration.Results;

[GenerateOneOf]
public sealed partial class MigrationResult : OneOfBase<Success, MigrationError>;
