namespace Shared.Events.Auth;

public record UserLoggedInEvent(
    Guid UserId,
    string Email,
    DateTime LoggedInAt);
