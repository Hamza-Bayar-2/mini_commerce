using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<UserRefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) : base(db) => _db = db;

    public async Task<UserRefreshToken?> GetByTokenAsync(string tokenHash, CancellationToken ct)
    {
        return await _db.UserRefreshTokens
        .FirstOrDefaultAsync(r => tokenHash == r.TokenHash, ct);
    }
}