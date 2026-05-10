using System.Reflection;
using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.Logout;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.PipelineBehaviors;
using AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

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

        services.AddScoped<ILoggingStrategy<LoginCommand, Result<AuthResponseDto>>, LoginLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<RegisterCommand, Result<RegisterResponseDto>>, RegisterLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<LogoutCommand, Result<Unit>>, LogoutLoggingStrategy>();
        services.AddScoped<ILoggingStrategy<RefreshTokenCommand, Result<AuthResponseDto>>, RefreshTokenLoggingStrategy>();

        return services;
    }
}