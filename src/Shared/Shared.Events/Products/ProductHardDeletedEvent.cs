namespace Shared.Events.Products;

public record ProductHardDeletedEvent(
    Guid ProductId,
    string Name,
    int Stock,
    DateTime DeletedAt);