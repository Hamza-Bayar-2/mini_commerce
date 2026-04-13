using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Persistence.Context;
using ProductService.Infrastructure.Persistence.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace ProductService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddHttpContextAccessor();

        // Database connection
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default"))
        );

        // Repositories connections
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository>(sp =>
            new CachedProductRepository(
                sp.GetRequiredService<ProductRepository>(),
                sp.GetRequiredService<IDistributedCache>(),
                sp.GetRequiredService<ILogger<CachedProductRepository>>()
        ));
        services.AddScoped<IProductStatusRepository, ProductStatusRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // RabbitMQ connections
        services.AddMassTransit(x =>
        {
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
            });
        });

        // Service connections
        services.AddScoped<IStatusService, ProductStatusService>();
        services.AddScoped<IProductService, ProductManagerService>();
        services.AddScoped<IEventPublisherService, EventPublisherService>();

        // Redis connection
        services.AddStackExchangeRedisCache(redisOpt =>
        {
            redisOpt.Configuration = configuration.GetConnectionString("Redis");
            redisOpt.InstanceName = "ProductService:";
        });

        return services;
    }
}