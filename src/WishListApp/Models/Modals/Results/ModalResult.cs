using OneOf;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class ModalResult<T> : OneOfBase<T, ModalCancelled>;

public sealed record ModalCancelled();
