using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Infrastructure.Persistence.Context;
using ProductService.Infrastructure.Persistence.Repositories;

namespace ProductService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string? connectionString)
    {
        services.AddHttpContextAccessor();
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductStatusRepository, ProductStatusRepository>();

        return services;
    }
}