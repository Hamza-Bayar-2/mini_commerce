using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using LogService.Infrastructure.Persistence.Context;

namespace LogService.Infrastructure.Persistence.Repositories;

public class LogRepository : GenericRepository<Log>, ILogRepository
{
    public LogRepository(LogDbContext db) : base(db)
    {

    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
