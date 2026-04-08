namespace AuthService.Domain.Entities;

public partial class UserCredential
{
    public Guid UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
