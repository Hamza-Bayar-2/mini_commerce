namespace Shared.Events.Auth;

public record TokenRefreshedEvent(
    Guid UserId,
    DateTime RefreshedAt);
