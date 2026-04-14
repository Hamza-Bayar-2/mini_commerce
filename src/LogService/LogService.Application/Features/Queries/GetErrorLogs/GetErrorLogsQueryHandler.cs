using LogService.Application.Common.Models;
using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Queries.GetErrorLogs;

public class GetErrorLogsQueryHandler : IRequestHandler<GetErrorLogsQuery, Result<IEnumerable<Log>>>
{
    private readonly ILogRepository _logRepo;

    public GetErrorLogsQueryHandler(ILogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<Result<IEnumerable<Log>>> Handle(GetErrorLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepo.FindAsync(x => x.Level == "ERROR", cancellationToken);
        return Result<IEnumerable<Log>>.Success(logs);
    }
}
