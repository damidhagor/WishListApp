namespace WishListApp.Data.Migration.Results;

[GenerateOneOf]
public sealed partial class MigrationResult : OneOfBase<Migrated, InvalidSourceVersion, InvalidDocument> { }
