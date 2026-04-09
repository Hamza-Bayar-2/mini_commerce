namespace AuthService.Application.DTOs;

public class AuthResponseDto
{
  public required string AccessToken { get; set; }

  public required string RefreshToken { get; set; }

  public required DateTime RefreshTokenExpiresAt { get; set; }
}

