using Autofac;

namespace Attribution.Domain
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Manager"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}