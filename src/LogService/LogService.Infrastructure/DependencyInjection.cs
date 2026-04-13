using LogService.Application.Interfaces.Repositories;
using LogService.Infrastructure.Consumers;
using LogService.Infrastructure.Persistence.Context;
using LogService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        services.AddDbContext<LogDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<ILogRepository, LogRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<ProductUpdatedConsumer>();
            x.AddConsumer<ProductDeletedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ReceiveEndpoint("log-service-product-events", e =>
                {
                    e.ConfigureConsumer<ProductCreatedConsumer>(ctx);
                    e.ConfigureConsumer<ProductUpdatedConsumer>(ctx);
                    e.ConfigureConsumer<ProductDeletedConsumer>(ctx);
                });
            });
        });

        return services;
    }
}