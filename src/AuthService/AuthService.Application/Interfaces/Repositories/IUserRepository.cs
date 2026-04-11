using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetUserInfoAsync(Guid userId, CancellationToken ct);
}
