using System.Collections.Generic;
using System.Reflection;
using Autofac;
using AutoMapper;

namespace Attribution.Configuration
{
    public static class ContainerBuilderExtensions
    {
        public static void AddAutoMapperProfiles(this ContainerBuilder builder, Assembly assemblyToScan)
        {
            builder.RegisterAssemblyTypes(assemblyToScan)
                .AssignableTo<Profile>()
                .As<Profile>()
                .SingleInstance();
        }

        public static void AddAutoMapper(this ContainerBuilder builder)
        {
            builder.Register(ctx =>
                {
                    var mapConfig = new MapperConfiguration(cfg =>
                    {
                        var profiles = ctx.Resolve<IEnumerable<Profile>>();
                        cfg.AddProfiles(profiles);
                    });
                    return new Mapper(mapConfig);
                })
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}