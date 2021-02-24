
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClientConsul.Configuration.Consul
{
    public static class ConsulStartup
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulOptions>(options => configuration.GetSection("consul").Bind(options));

            var consulConfig = services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Options.IOptions<ConsulOptions>>().Value;

            services.AddSingleton(consulConfig);
            services.AddHealthChecks();
            services.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
           {
               if (!string.IsNullOrEmpty(consulConfig.Host))
               {
                   cfg.Address = new Uri(consulConfig.Host);
               }
           }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var consulOptions = scope.ServiceProvider.GetRequiredService<ConsulOptions>();
            var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

            if (!consulOptions.Enabled)
                return app;

            var serviceId = !string.IsNullOrEmpty(consulOptions.Id) ? consulOptions.Id : Guid.NewGuid().ToString();
            var consulServiceId = $"{consulOptions.Service}:{serviceId}";

            var client = scope.ServiceProvider.GetRequiredService<IConsulClient>();

            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = consulOptions.Service,
                ID = consulServiceId,
                Address = consulOptions.Address,
                Port = consulOptions.Port,
                Tags = consulOptions.Tags,
                Meta = consulOptions.MetaData
            };

            if (consulOptions.PingEnabled)
            {
                var healthService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();

                if (healthService != null)
                {
                    var scheme =
                        consulOptions.Address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                        ? string.Empty
                        : "http://";

                    var check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        HTTP = $"{scheme}{consulOptions.Address}{(consulOptions.Port > 0 ? consulOptions.Port : string.Empty)}/health"
                    };

                    consulServiceRegistration.Checks = new[] { check };
                }
                else
                {
                    throw new ConsulConfigurationException("Verifique os parametros de configuração do consul");
                }

            }
            client.Agent.ServiceRegister(consulServiceRegistration);

            lifetime.ApplicationStopping.Register(() => client.Agent.ServiceDeregister(consulServiceRegistration.ID).ConfigureAwait(true));

            return app;
        }
    }
}
