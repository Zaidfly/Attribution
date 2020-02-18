using System;
using System.Reflection;
using Attribution.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Attribution.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile("settings.json", false, true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .UseYouDoLogging()
                .UseStartup<Startup>()
                .Build();

            OnStarting(host);

            host.Run();
        }
        
        private static void OnStarting(IWebHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            var hostingEnvironment = host.Services.GetRequiredService<IHostingEnvironment>();

            var serviceName = Assembly.GetExecutingAssembly().GetName().Name;
            var environmentName = hostingEnvironment.EnvironmentName;

            logger.LogInformation(@"Starting: {serviceName}. Environment: {environmentName}.", serviceName, environmentName);
        }
    }
}