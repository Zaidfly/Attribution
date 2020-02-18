using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Exceptional;
using StackExchange.Exceptional;
using StackExchange.Exceptional.Stores;

namespace Attribution.Configuration
{
    public static class HostBuilderExtensions
    {
        private static Assembly EntryAssembly => Assembly.GetEntryAssembly();
        private static string EntryAssemblyName => EntryAssembly.GetName().Name;
        
        public static IHostBuilder UseYouDoLogging(this IHostBuilder builder)
        {
            return builder.ConfigureLogging((context, loggerBuilder) =>
            {
                loggerBuilder.ClearProviders();

                Exceptional.Configure(settings =>
                {
                    settings.AppendFullStackTraces = true;
                    settings.DefaultStore =
                        new SQLErrorStore(context.Configuration["Exceptional"], EntryAssemblyName);
                });
                
                var logger = GetLogger(EntryAssemblyName);

                loggerBuilder.AddSerilog(logger);
            });
        }
        
        public static IWebHostBuilder UseYouDoLogging(this IWebHostBuilder builder)
        {
            return builder.ConfigureLogging((context, loggerBuilder) =>
            {
                loggerBuilder.ClearProviders();

                Exceptional.Configure(settings =>
                {
                    settings.AppendFullStackTraces = true;
                    settings.DefaultStore =
                        new SQLErrorStore(context.Configuration["Exceptional"], EntryAssemblyName);
                });

                var logger = GetLogger(EntryAssemblyName);

                loggerBuilder.AddSerilog(logger);
            });
        }

        private static Logger GetLogger(string source) =>
            new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Source", source)                
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("MicroElements", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .WriteTo.Console(new JsonFormatter(renderMessage: true), LogEventLevel.Verbose)
                .WriteTo.Exceptional(LogEventLevel.Warning, category: source)
                .CreateLogger();
    }
}