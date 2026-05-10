namespace Shared.Events.Auth;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    DateTime RegisteredAt);
