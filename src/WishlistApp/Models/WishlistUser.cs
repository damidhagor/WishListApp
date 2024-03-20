namespace WishlistApp.Models;

public sealed record class WishlistUser(
    string Identifier,
    string Name,
    string[] Roles,
    (string Type, string Value)[] Claims);