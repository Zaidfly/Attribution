using Attribution.Configuration;
using Autofac;
using Module = Autofac.Module;

namespace Attribution.UserActionService
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