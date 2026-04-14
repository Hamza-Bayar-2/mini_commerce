using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Errors;

namespace LogService.Infrastructure.Consumers;

public class ErrorOccurredConsumer : IConsumer<ErrorOccurredEvent>
{
    private readonly ILogRepository _logRepo;

    public ErrorOccurredConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<ErrorOccurredEvent> context)
    {
        var log = new Log
        {
            ServiceName = context.Message.ServiceName,
            Message = $"{context.Message.Message} | Path: {context.Message.RequestPath} | Method: {context.Message.RequestMethod} | StackTrace: {context.Message.StackTrace}",
            Level = "ERROR",
            CreatedAt = context.Message.CreatedAt
        };

        await _logRepo.AddAsync(log, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
