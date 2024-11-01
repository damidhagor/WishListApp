using BlazorDialogs.Models.Results;
using OneOf;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class EditWishListItemResult : OneOfBase<Edited, Cancelled>;

public sealed record Edited();
