namespace Shared.Events.Auth;

public record UserLoggedOutEvent(
    Guid UserId,
    DateTime LoggedOutAt);
