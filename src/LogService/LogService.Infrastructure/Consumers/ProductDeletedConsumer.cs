using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MassTransit;
using Shared.Events.Products;

namespace LogService.Infrastructure.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
{
    private readonly ILogRepository _logRepo;

    public ProductDeletedConsumer(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        var msg = context.Message;

        await _logRepo.AddAsync(new Log
        {
            Message = $"Product deleted/soft-deleted: {msg.Name} (Id: {msg.ProductId})",
            ServiceName = "ProductService",
            CreatedAt = msg.DeletedAt
        }, context.CancellationToken);

        await _logRepo.SaveChangesAsync();
    }
}
