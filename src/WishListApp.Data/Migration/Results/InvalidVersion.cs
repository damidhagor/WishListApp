namespace WishListApp.Data.Migration.Results;

public sealed record InvalidVersion(uint SupportedVersion, uint Version);
