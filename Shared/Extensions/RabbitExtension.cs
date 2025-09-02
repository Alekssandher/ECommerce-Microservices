using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions
{
    public static class RabbitExtension
    {
        public static void AddRabbit(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator> configureBus = null!)
        {
            var rabbitSection = configuration.GetSection("RabbitMq") ?? throw new Exception("Make Sure to Set the RabbitMq params url in appsettings.json");
            var url = rabbitSection.GetValue<string>("url")?? throw new Exception("Make Sure to Set the RabbitMq param: url in appsettings.json");
            var user = rabbitSection.GetValue<string>("user")?? throw new Exception("Make Sure to Set the RabbitMq param: user in appsettings.json");
            var password = rabbitSection.GetValue<string>("password")?? throw new Exception("Make Sure to Set the RabbitMq param: password in appsettings.json");

            services.AddMassTransit(busConfigurator =>
            {
                configureBus?.Invoke(busConfigurator);

                busConfigurator.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(url), host =>
                    {
                        host.Username(user);
                        host.Password(password);
                    });
                    
                    cfg.ConfigureEndpoints(ctx);

                });
            });
        }
    }
}