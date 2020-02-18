using System;
using System.Threading.Tasks;
using Attribution.Configuration;
using Attribution.Configuration.RabbitMq;
using Autofac;
using Coworking.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Attribution.UserActionService.Processors;
using Autofac.Extensions.DependencyInjection;
using Coworking.Extensions;
using RabbitLink.Topology;

namespace Attribution.UserActionService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile("settings.json", false, true)
                        .AddEnvironmentVariables();
                })
                .UseYouDoLogging()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    var configuration = context.Configuration;
                    
                    builder.Register(c => configuration)
                        .As<IConfiguration>()
                        .SingleInstance();

                    builder.AddRabbit(configuration);
                    builder.AddDatabaseConnectionString(configuration);
                    
                    builder
                        .AddCoworking(context.Configuration.GetSection("Coworking").Get<CoworkingConfiguration>())
                        .AddMessageProcessor<UserRegisteredProcessor>(options =>
                        {
                            options.Exchange = "youdo.events";
                            options.RoutingKey = "user.created";
                            options.Queue = "attribution.useractions.user.created";
                            options.ExchangeType = LinkExchangeType.Direct;
                            options.Exclusive = false;
                        })
                        .AddMessageProcessor<TaskCreatedProcessor>(options =>
                        {
                            options.Exchange = "youdo.events";
                            options.RoutingKey = "task.created";
                            options.Queue = "attribution.useractions.task.created";
                            options.ExchangeType = LinkExchangeType.Direct;
                            options.Exclusive = false;
                        })
                        .AddMessageProcessor<TaskOfferCreatedProcessor>(options =>
                        {
                            options.Exchange = "youdo.events";
                            options.RoutingKey = "taskoffer.created";
                            options.Queue = "attribution.useractions.taskoffer.created";
                            options.ExchangeType = LinkExchangeType.Direct;
                            options.Exclusive = false;
                        })
                        .AddMessageProcessor<TrackingVisitEventProcessor>(options =>
                        {
                            options.Exchange = "youdo.attribution.events";
                            options.RoutingKey = "tracking.visit";
                            options.Queue = "attribution.tracking.visit";
                            options.ExchangeType = LinkExchangeType.Direct;
                            options.Exclusive = false;
                        });
                    
                    builder.RegisterModule<IocModule>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}