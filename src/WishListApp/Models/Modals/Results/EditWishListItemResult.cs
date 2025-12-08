using OneOf;
using Shared.Blazor.Dialogs.Models.Results;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class EditWishListItemResult : OneOfBase<Edited, Cancelled>;

public sealed record Edited();
