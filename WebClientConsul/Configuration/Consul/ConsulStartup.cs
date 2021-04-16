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

            var consulServiceRegistration = new AgentServiceRegistration
            {
                ID = consulOptions.Id,
                Name = consulOptions.Service,
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
                    var check = new AgentCheckRegistration()
                    {
                        HTTP = $"http://{consulOptions.Address}/health",
                        Notes = "Checks /health/status on localhost",
                        Timeout = TimeSpan.FromSeconds(3),
                        Interval = TimeSpan.FromSeconds(10),
                        DockerContainerID = System.Environment.MachineName
                    };

                    consulServiceRegistration.Checks = new[] { check };
                }
                else
                {
                    throw new ConsulConfigurationException("Verifique os parametros de configuração do consul");
                }
            }
            client.Agent.ServiceRegister(consulServiceRegistration);
            lifetime.ApplicationStopping.Register(() => client.Agent.ServiceDeregister(consulServiceRegistration.ID).Wait());

            return app;
        }
    }
}
