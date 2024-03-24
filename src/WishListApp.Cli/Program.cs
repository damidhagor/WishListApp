using System.CommandLine;
using WishListApp.Cli.Database;

var rootCommand = new RootCommand("Cli tool for the WishList App");

rootCommand.AddDatabaseCommand();

await rootCommand.InvokeAsync(args);
