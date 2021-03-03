using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;

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
               cfg.Address = new Uri(consulConfig.Host);
           }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var consulOptions = scope.ServiceProvider.GetRequiredService<ConsulOptions>();
            var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
            var client = scope.ServiceProvider.GetRequiredService<IConsulClient>();

            if (!consulOptions.Enabled)
                return app;

            if (string.IsNullOrEmpty(consulOptions.Service))
                throw new ConsulConfigurationException("Service deve ter um nome");

            if (consulOptions.Port == 0)
                throw new ConsulConfigurationException("Service deve ter uma porta");

            if (string.IsNullOrEmpty(consulOptions.Address))
                throw new ConsulConfigurationException("Service deve ter um Address");

            var consulServiceId = $"{consulOptions.Service}:{consulOptions.Id}";

            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = consulOptions.Service,
                ID = consulServiceId,
                Address = consulOptions.Address,
                Port = consulOptions.Port,
                Tags = consulOptions.Tags,
                Meta = consulOptions.MetaData,
            };

            if (consulOptions.PingEnabled)
            {
                var healthService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();

                if (healthService != null)
                {
                    var scheme =
                        consulOptions.Address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                        ? string.Empty
                        : "https://";
                    
                    var test = $"{scheme}{consulOptions.Address}:{(consulOptions.Port > 0 ? consulOptions.Port : string.Empty)}/health";

                    var checkHTTP = new AgentServiceCheck
                    {
                        
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        HTTP = $"{scheme}{consulOptions.Address}{(consulOptions.Port > 0 ? consulOptions.Port : string.Empty)}/health"
                    };

                    var checkFTP = new AgentServiceCheck 
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        TCP = "localhost:5001"
                    };

                    consulServiceRegistration.Checks = new[] {  checkFTP };
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
