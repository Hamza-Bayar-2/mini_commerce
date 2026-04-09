namespace AuthService.Domain.Entities;

public class Role
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation property
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
}