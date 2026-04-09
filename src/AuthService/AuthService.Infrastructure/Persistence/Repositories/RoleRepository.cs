using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
  private readonly AppDbContext _db;
  public RoleRepository(AppDbContext db) : base(db) => _db = db;

  public async Task<List<Role>> GetRolesByIdsAsync(List<short> roleIds, CancellationToken ct)
  {
    return await _db.Roles
    .Where(r => roleIds.Contains(r.Id))
    .ToListAsync(ct);
  }

  public async Task<List<Role>> GetRolesByNamesAsync(List<string> roleNames, CancellationToken ct = default)
  {
    return await _db.Roles
        .Where(r => roleNames.Contains(r.Name))
        .ToListAsync(ct);
  }
}