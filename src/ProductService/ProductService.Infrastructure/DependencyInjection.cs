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
                sp.GetRequiredService<IDistributedCache>()
        ));
        services.AddScoped<IProductStatusRepository, ProductStatusRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Service connections
        services.AddScoped<IStatusService, ProductStatusService>();
        services.AddScoped<IProductService, ProductManagerService>();

        // Redis connection
        services.AddStackExchangeRedisCache(redisOpt =>
        {
            redisOpt.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}