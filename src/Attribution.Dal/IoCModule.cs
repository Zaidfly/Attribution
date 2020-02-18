using System;
using Attribution.Configuration;
using Attribution.Dal.Redis;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YouDo.Caching;
using YouDo.Caching.Redis;
using Module = Autofac.Module;

namespace Attribution.Dal
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AttributionDb>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
            
            RegisterRedisDependencies(builder);

            builder.AddAutoMapperProfiles(ThisAssembly);
        }

        private static void RegisterRedisDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<RedisDistributedCache>()
                .As<IDistributedCache>()
                .SingleInstance();

            builder.Register(ctx =>
                {
                    var config = ctx.Resolve<IConfiguration>();
                    return ConnectionMultiplexer.Connect(config.GetSection("redis").Value);
                })
                .As<IConnectionMultiplexer>()
                .SingleInstance();

            builder.RegisterType<RedisInvalidationProducer>()
                .As<IInvalidationFallback>()
                .SingleInstance();

            builder.Register(_ => Options.Create(new RedisCircuitBreakerOptions
                {
                    ExceptionsAllowedBeforeBreaking = 5,
                    DurationOfBreak = TimeSpan.FromSeconds(10)
                })).As<IOptions<RedisCircuitBreakerOptions>>()
                .SingleInstance();
        }
    }
}