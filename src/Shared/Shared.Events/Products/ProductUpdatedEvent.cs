namespace Shared.Events.Products;

public record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    int Stock,
    DateTime UpdatedAt);