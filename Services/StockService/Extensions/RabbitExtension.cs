using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using StockService.Consumers;

namespace StockService.Extensions
{
    internal static class RabbitExtension
    {
        public static void AddRabbit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSection = configuration.GetSection("RabbitMq");
            var url = rabbitSection.GetValue<string>("url")?? throw new Exception("Make Sure to Set the RabbitMq params in appsettings.json");
            var user = rabbitSection.GetValue<string>("user")?? throw new Exception("Make Sure to Set the RabbitMq params in appsettings.json");
            var password = rabbitSection.GetValue<string>("password")?? throw new Exception("Make Sure to Set the RabbitMq params in appsettings.json");

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<SaleCreatedConsumer>();

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