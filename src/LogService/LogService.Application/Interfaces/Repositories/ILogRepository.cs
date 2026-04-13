using LogService.Domain.Entities;

namespace LogService.Application.Interfaces.Repositories;

public interface ILogRepository : IGenericRepository<Log>
{
    Task SaveChangesAsync();
}
