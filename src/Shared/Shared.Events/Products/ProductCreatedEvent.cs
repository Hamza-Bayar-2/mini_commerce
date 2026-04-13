namespace Shared.Events.Products;

public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    int Stock,
    DateTime CreatedAt);