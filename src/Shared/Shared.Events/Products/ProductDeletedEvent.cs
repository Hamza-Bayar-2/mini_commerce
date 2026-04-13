namespace Shared.Events.Products;

public record ProductDeletedEvent(
    Guid ProductId,
    string Name,
    int Stock,
    DateTime DeletedAt);