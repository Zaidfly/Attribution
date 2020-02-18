using System;
using System.Reflection;
using Attribution.Configuration.RabbitMq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YouDo.Microservices.Core.Extensions;

namespace Attribution.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddYouDoDefaultMvcServices(Assembly.GetExecutingAssembly().GetName().Name, true)
                .AddFluentValidation(o => o.RegisterValidatorsFromAssemblyContaining<Startup>());
                
            services.AddHealthChecks();
            
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.Register(c => _configuration)
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.AddRabbit(_configuration);
            builder.AddDatabaseConnectionString(_configuration);
            
            builder.RegisterModule<IocModule>();
            
            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/hc");
            app.UseYouDoMvc(provider);
        }
    }
}