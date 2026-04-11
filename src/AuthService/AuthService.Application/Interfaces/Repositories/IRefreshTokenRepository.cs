using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<UserRefreshToken>
{
    Task<UserRefreshToken?> GetByTokenAsync(string tokenHash, CancellationToken ct);
}