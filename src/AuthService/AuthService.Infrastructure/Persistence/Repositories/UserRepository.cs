using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) : base(db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
          => await _db.Users
              .Include(u => u.UserCredential)
              .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetUserInfoAsync(Guid userId, CancellationToken ct)
          => await _db.Users
              .Include(u => u.Roles)
              .FirstOrDefaultAsync(u => u.Id == userId, ct);
}