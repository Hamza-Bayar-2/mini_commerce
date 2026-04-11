using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<List<Role>> GetRolesByNamesAsync(List<string> roleNames, CancellationToken ct);
    Task<List<Role>> GetRolesByIdsAsync(List<short> roleIds, CancellationToken ct);
}