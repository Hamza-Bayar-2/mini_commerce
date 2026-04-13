using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Products;

namespace LogService.Infrastructure.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
{
    private readonly ILogRepository _logRepo;

    public ProductUpdatedConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        var msg = context.Message;

        await _logRepo.AddAsync(new Log
        {
            Message = $"Product updated: {msg.Name} (Id: {msg.ProductId}). New Stock: {msg.Stock}",
            ServiceName = "ProductService",
            CreatedAt = msg.UpdatedAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
