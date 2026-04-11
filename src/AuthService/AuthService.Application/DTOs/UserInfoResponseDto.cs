namespace AuthService.Application.DTOs;

public class UserInfoResponseDto

{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required List<string> Roles { get; set; }
};