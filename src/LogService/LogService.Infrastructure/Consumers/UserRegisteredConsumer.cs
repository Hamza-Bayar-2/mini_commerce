using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Auth;

namespace LogService.Infrastructure.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ILogRepository _logRepo;

    public UserRegisteredConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;
        await _logRepo.AddAsync(new Log
        {
            Message = $"User registered: {msg.FullName} ({msg.Email}). Id: {msg.UserId}",
            ServiceName = "AuthService",
            CreatedAt = msg.RegisteredAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
