using System.CommandLine;
using WishlistApp.Cli.Database.Commands;
using WishlistApp.Cli.Database.Options;

namespace WishlistApp.Cli.Database;

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
