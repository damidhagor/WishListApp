using OneOf;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class ConfirmationResult : OneOfBase<Confirmed, Cancelled>;

public sealed record Confirmed();
