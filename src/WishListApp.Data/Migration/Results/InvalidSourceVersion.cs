namespace WishListApp.Data.Migration.Results;

public sealed record InvalidSourceVersion(uint ExpectedVersion, uint ActualVersion);
