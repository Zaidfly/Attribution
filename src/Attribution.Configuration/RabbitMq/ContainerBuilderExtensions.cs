using System;
using System.Diagnostics;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Serialization.Json;

namespace Attribution.Configuration.RabbitMq
{
    public static class ContainerBuilderExtensions
    {
        public static void AddDatabaseConnectionString(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(new DbConnectionString(configuration.GetSection("AttributionDb").Value))
                .AsSelf()
                .SingleInstance();
        }
        
        public static void AddRabbit(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance(configuration.GetSection("RabbitMq").Get<RabbitMqConfig>())
                .AsSelf()
                .SingleInstance();
            
            builder.Register(ctx =>
            {
                var id = Guid.NewGuid();
                var config = ctx.Resolve<RabbitMqConfig>();
                string ret;

                using (var proc = Process.GetCurrentProcess())
                {
                    var exeName = Path.GetFileName(proc.MainModule.FileName);

                    ret = $"PID: {proc.Id}, Exe: \"{exeName}\", InstanceId: \"{id}\"";
                }

                return LinkBuilder.Configure
                    .Uri(config.Url)
                    .AppId($"attribution.{id}")
                    .ConnectionName(ret)
                    .Timeout(TimeSpan.FromSeconds(config.ConnectionTimeout))
                    .RecoveryInterval(TimeSpan.FromSeconds(config.RecoveryInterval))
                    .LoggerFactory(new LinkLoggerFactory(ctx.Resolve<ILoggerFactory>()))
                    .Serializer(new LinkJsonSerializer())
                    .Build();
                
            }).As<ILink>().SingleInstance();
        }
    }
}