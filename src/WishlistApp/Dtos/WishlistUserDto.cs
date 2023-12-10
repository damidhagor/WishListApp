namespace WishlistApp.Dtos;

public sealed record class WishlistUserDto(
    string Identifier,
    string Name,
    string[] Roles,
    (string Type, string Value)[] Claims);