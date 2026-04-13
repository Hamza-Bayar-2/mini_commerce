using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Auth;

namespace LogService.Infrastructure.Consumers;

public class UserLoggedInConsumer : IConsumer<UserLoggedInEvent>
{
    private readonly ILogRepository _logRepo;

    public UserLoggedInConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<UserLoggedInEvent> context)
    {
        var msg = context.Message;
        await _logRepo.AddAsync(new Log
        {
            Message = $"User logged in: {msg.Email}. Id: {msg.UserId}",
            ServiceName = "AuthService",
            CreatedAt = msg.LoggedInAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
