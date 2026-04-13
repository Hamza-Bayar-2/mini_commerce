using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Products;

namespace LogService.Infrastructure.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ILogRepository _logRepo;

    public ProductCreatedConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var msg = context.Message;

        await _logRepo.AddAsync(new Log
        {
            Message = $"Product created: {msg.Name} (Id: {msg.ProductId})",
            ServiceName = "ProductService",
            CreatedAt = msg.CreatedAt
        }, CancellationToken.None);

        await _logRepo.SaveChangesAsync();
    }
}