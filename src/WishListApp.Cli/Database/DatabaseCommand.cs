using System.CommandLine;
using WishListApp.Cli.Database.Commands;
using WishListApp.Cli.Database.Options;

namespace WishListApp.Cli.Database;

internal static class DatabaseCommand
{
    public static RootCommand AddDatabaseCommand(this RootCommand rootCommand)
    {
        var databaseCommand = new Command("database", "Perform operations on the database");

        var connectionStringOption = databaseCommand.AddConnectionStringOption();

        databaseCommand.AddApplyMigrationCommand(connectionStringOption);

        rootCommand.AddCommand(databaseCommand);

        return rootCommand;
    }
}
