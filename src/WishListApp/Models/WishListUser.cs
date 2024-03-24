namespace WishListApp.Models;

public sealed record class WishListUser(
    string Identifier,
    string Name,
    string[] Roles,
    (string Type, string Value)[] Claims);