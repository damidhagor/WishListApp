namespace WishListApp.ProductCrawling.Models;

public sealed record class ProductInformation(
    string? Title,
    string? Description,
    string? ImageUrl,
    decimal? Price,
    string? Currency);
