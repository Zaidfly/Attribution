using System.Reflection;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EnvironmentInitializer
{
    internal static class DatabaseMigrator
    {
        internal static async Task InitializeDatabaseAsync(string dbConnectionString, Assembly assemblyToScan)
        {
            await CreateDatabaseIfNecessaryAsync(dbConnectionString);

            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(dbConnectionString)
                    .ScanIn(assemblyToScan)
                    .For.Migrations())
                .Configure<RunnerOptions>(opt =>
                {
                    opt.Tags = new []{"Pre"};
                    opt.TransactionPerSession = true;
                })
                .BuildServiceProvider(false);;

            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                runner.MigrateUp();
            }
        }
        
        internal static void SeedDatabase(string dbConnectionString, Assembly assemblyToScanForMigrations)
        {
            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(dbConnectionString)
                    .ScanIn(assemblyToScanForMigrations)
                    .For.Migrations())
                .Configure<RunnerOptions>(opt =>
                {
                    opt.TransactionPerSession = true;
                })
                .BuildServiceProvider(false);

            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                runner.MigrateUp();
            }
        }
        
        internal static async Task DropDatabaseAsync(string dbConnectionString)
        {
            var dbName = new NpgsqlConnectionStringBuilder(dbConnectionString).Database;
            var csBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString) {Database = "postgres"};
            using (var conn = new NpgsqlConnection(csBuilder.ToString()))
            {
                conn.Open();
                var dbExists = await CheckDbExistsAsync(conn, dbName);

                if (!dbExists) return;

                var dropDbSql = $@"DROP DATABASE ""{dbName}"";";
                using (var cmd = new NpgsqlCommand(dropDbSql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private static async Task CreateDatabaseIfNecessaryAsync(string dbConnectionString)
        {
            var dbName = new NpgsqlConnectionStringBuilder(dbConnectionString).Database;
            var csBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString) {Database = "postgres"};
            using (var conn = new NpgsqlConnection(csBuilder.ToString()))
            {
                await conn.OpenAsync();
                var dbExists = await CheckDbExistsAsync(conn, dbName);

                if (dbExists) return;

                var createDbSql = $@"CREATE DATABASE ""{dbName}"" WITH OWNER = {csBuilder.Username};";
                using (var cmd = new NpgsqlCommand(createDbSql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private static async Task<bool> CheckDbExistsAsync(NpgsqlConnection conn, string dbName)
        {
            var checkExistsSql = $"SELECT 1 FROM pg_database WHERE datname='{dbName}'";
            using (var cmd = new NpgsqlCommand(checkExistsSql, conn))
            {
                return await cmd.ExecuteScalarAsync() != null;
            }
        }
    }
}