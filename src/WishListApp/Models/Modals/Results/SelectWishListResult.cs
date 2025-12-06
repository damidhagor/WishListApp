using OneOf;
using Shared.Blazor.Dialogs.Models.Results;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class SelectWishListResult : OneOfBase<Selected, Cancelled>;

public sealed record Selected(WishList List);
