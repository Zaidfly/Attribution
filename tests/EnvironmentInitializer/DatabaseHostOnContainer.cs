using System;
using System.Reflection;
using System.Threading.Tasks;
using Docker.DotNet;

namespace EnvironmentInitializer
{
    public class DatabaseHostOnContainer : IDisposable
    {
        private readonly PostgresContainer _postgresContainer;
        private readonly string _dbConnectionString;
        private readonly Assembly _preMigrationsAssembly;
        private readonly Assembly _seedMigrationsAssembly;
        
        public DatabaseHostOnContainer(
            string dbConnectionString,
            Assembly preMigrationsAssembly,
            Assembly seedMigrationsAssembly)
        {
            _dbConnectionString = dbConnectionString;
            _preMigrationsAssembly = preMigrationsAssembly;
            _seedMigrationsAssembly = seedMigrationsAssembly;
            
            var dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
            _postgresContainer = new PostgresContainer(dockerClient);
        }

        public async Task StartAsync()
        {
            await _postgresContainer.StartAsync();
            
            await DatabaseMigrator.DropDatabaseAsync(_dbConnectionString);
            await DatabaseMigrator.InitializeDatabaseAsync(_dbConnectionString, _preMigrationsAssembly);
            DatabaseMigrator.SeedDatabase(_dbConnectionString, _seedMigrationsAssembly);
        }

        public async Task StopAsync()
        {
            await _postgresContainer.RemoveAsync();
        }
        
        public void Dispose()
        {
            _postgresContainer.Dispose();
        }
    }
}