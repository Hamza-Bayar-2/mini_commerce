using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<UserRefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) : base(db) => _db = db;
}