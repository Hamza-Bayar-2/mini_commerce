using LogService.Application.Common.Models;
using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Queries.GetInfoLogs;

public class GetInfoLogsQueryHandler : IRequestHandler<GetInfoLogsQuery, Result<IEnumerable<Log>>>
{
    private readonly ILogRepository _logRepo;

    public GetInfoLogsQueryHandler(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<Result<IEnumerable<Log>>> Handle(GetInfoLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepo.FindAsync(x => x.Level == "INFO", cancellationToken);
        return Result<IEnumerable<Log>>.Success(logs);
    }
}
