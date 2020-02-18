using Attribution.Configuration;
using Autofac;
using AutoMapper;

namespace Attribution.Api
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Dal.IocModule>();
            builder.RegisterModule<Domain.IocModule>();
            builder.AddAutoMapper();
            builder.AddAutoMapperProfiles(ThisAssembly);
        }
    }
}