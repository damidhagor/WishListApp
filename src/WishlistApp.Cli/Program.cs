using System.CommandLine;
using WishlistApp.Cli.Database;

var rootCommand = new RootCommand("Cli tool for the Wishlist App");

rootCommand.AddDatabaseCommand();

await rootCommand.InvokeAsync(args);
