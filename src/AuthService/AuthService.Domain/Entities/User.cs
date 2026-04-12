namespace AuthService.Domain.Entities;

public partial class User
{
    public User()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    /// <summary>
    /// Giriş anahtarı
    /// </summary>
    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }


    /// <summary>
    /// Soft delete uygulanan kayıtların silinme anı
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserCredential? UserCredential { get; set; }

    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
