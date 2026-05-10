namespace AuthService.Application.DTOs;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpiresAt { get; set; }
}

