using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WishListApp.Cli.Database.Binders;
using WishListApp.Cli.Shared.Binders;
using WishListApp.Data.Sql;

namespace WishListApp.Cli.Database.Commands;

internal static class ApplyMigrationCommand
{
    public static Command AddApplyMigrationCommand(this Command parentCommand, Option<string> connectionStringOption)
    {
        var migrationCommand = new Command("apply-migrations", "Apply migrations to the database");

        migrationCommand.SetHandler(ApplyMigrations, new LoggerBinder(), new WishListDbContextBinder(connectionStringOption));

        parentCommand.AddCommand(migrationCommand);
        return parentCommand;
    }

    private static async Task ApplyMigrations(ILogger logger, WishListDbContext context)
    {
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();

        if (pendingMigrations.Length == 0)
        {
            logger.LogInformation("Found no pending migrations.");
            return;
        }

        logger.LogInformation("Found {count} pending migrations.", pendingMigrations.Length);

        await context.Database.MigrateAsync();

        logger.LogInformation("Applied all pending migrations.");
    }
}
