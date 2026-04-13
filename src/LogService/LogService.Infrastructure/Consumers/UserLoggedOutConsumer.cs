using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Auth;

namespace LogService.Infrastructure.Consumers;

public class UserLoggedOutConsumer : IConsumer<UserLoggedOutEvent>
{
    private readonly ILogRepository _logRepo;

    public UserLoggedOutConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<UserLoggedOutEvent> context)
    {
        var msg = context.Message;
        await _logRepo.AddAsync(new Log
        {
            Message = $"User logged out. Id: {msg.UserId}",
            ServiceName = "AuthService",
            CreatedAt = msg.LoggedOutAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
