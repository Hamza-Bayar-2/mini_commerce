using System.Reflection;
using ProductService.Application.PipelineBehaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;
using ProductService.Application.Features.Product.Commands.CreateProduct;
using ProductService.Application.Features.Product.Commands.UpdateProduct;
using ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;
using ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;

namespace ProductService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviors<,>));
        });

        services.AddScoped<ILoggingStrategy<CreateProductCommand, Result<ProductResponseDto>>, CreateProductLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<UpdateProductCommand, Result<ProductResponseDto>>, UpdateProductLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<SoftDeleteProductCommand, Result<ProductResponseDto>>, SoftDeleteProductLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<HardDeleteProductCommand, Result<ProductResponseDto>>, HardDeleteProductLoggingStrategy>();

        return services;
    }
}