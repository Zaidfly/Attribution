using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Attribution.Dal.Repositories;
using EnvironmentInitializer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Attribution.Migrations;
using Xunit;
using YouDo.Caching;

namespace Attribution.Api.Tests
{
    public class AttributionApiTestsFixture : IDisposable, IAsyncLifetime
    {
        private const string DbConnectionString =
            "Host=localhost;Port=4848;Database=AttributionDbInt;Username=p;Password=p";

        private readonly DatabaseHostOnContainer _databaseHost;

        public readonly TestServer TestServer;

        public AttributionApiTestsFixture()
        {
            var migrationsAssembly = typeof(Migration20190607174119_CreateTables).Assembly;
            
            _databaseHost = new DatabaseHostOnContainer(
                DbConnectionString,
                migrationsAssembly,
                Assembly.GetExecutingAssembly());
            
            var hostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(config =>
                {
                    config
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"AttributionDb", DbConnectionString},
                            {"Redis", "ghost.local.com:6379,syncTimeout=5000,connectTimeout=10000,allowAdmin=true,abortConnect=false"},
                            {"RabbitMq:Url", "amqp://y:y@ghost.local.com"},
                            {"RabbitMq:ConnectionTimeout", "10"},
                            {"RabbitMq:RecoveryInterval", "3"},
                        });
                })
                .ConfigureLogging((context, builder) => { builder.AddDebug(); })
                .UseStartup<Startup>();

            TestServer = new TestServer(hostBuilder);
        }

        public async Task InitializeAsync()
        {
            await _databaseHost.StartAsync();
            await ClearCache();
        }

        public Task DisposeAsync()
        {
            return _databaseHost.StopAsync();
        }

        public void Dispose()
        {
            _databaseHost?.Dispose();
            TestServer?.Dispose();
        }

        private Task ClearCache()
        {
            using (var scope = TestServer.Host.Services.CreateScope())
            {
                var redis = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                return redis.DeleteAsync(UrlReferrerRepository.WhiteListCacheKey);
            }
        }
    }
}