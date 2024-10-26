using OneOf;

namespace WishListApp.Models.Modals.Results;

[GenerateOneOf]
public sealed partial class TextInputResult : OneOfBase<TextInput, Cancelled>;

public sealed record TextInput(string Text);
