namespace Shared.Events.Auth;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FullName,
    DateTime RegisteredAt);
