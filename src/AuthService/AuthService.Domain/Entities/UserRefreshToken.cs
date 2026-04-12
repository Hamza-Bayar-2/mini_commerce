namespace AuthService.Domain.Entities;

public partial class UserRefreshToken
{
    public UserRefreshToken()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? ClientIp { get; set; }

    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Güvenlik nedeniyle iptal edildi mi?
    /// </summary>
    public bool? IsRevoked { get; set; }

    /// <summary>
    /// Bu token yerine üretilen yeni token id
    /// </summary>
    public Guid? ReplacedBy { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserRefreshToken> InverseReplacedByNavigation { get; set; } = new List<UserRefreshToken>();

    public virtual UserRefreshToken? ReplacedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
