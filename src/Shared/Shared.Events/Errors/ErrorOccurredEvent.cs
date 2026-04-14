namespace Shared.Events.Errors;

public record ErrorOccurredEvent(
    string ServiceName,
    string Message,
    string? StackTrace,
    DateTime CreatedAt,
    string? RequestPath,
    string? RequestMethod
);
