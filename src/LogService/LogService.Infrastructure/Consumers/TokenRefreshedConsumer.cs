using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Auth;

namespace LogService.Infrastructure.Consumers;

public class TokenRefreshedConsumer : IConsumer<TokenRefreshedEvent>
{
    private readonly ILogRepository _logRepo;

    public TokenRefreshedConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<TokenRefreshedEvent> context)
    {
        var msg = context.Message;
        await _logRepo.AddAsync(new Log
        {
            Message = $"Token refreshed for User. Id: {msg.UserId}",
            ServiceName = "AuthService",
            CreatedAt = msg.RefreshedAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
