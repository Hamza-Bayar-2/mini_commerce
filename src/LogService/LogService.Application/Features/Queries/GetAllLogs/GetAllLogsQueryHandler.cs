using LogService.Application.Common.Models;
using LogService.Application.Interfaces.Repositories;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Logs.Queries.GetAllLogs;

public sealed class GetAllLogsQueryHandler : IRequestHandler<GetAllLogsQuery, Result<IEnumerable<Log>>>
{
    private readonly ILogRepository _logRepository;

    public GetAllLogsQueryHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<Result<IEnumerable<Log>>> Handle(GetAllLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        return Result<IEnumerable<Log>>.Success(logs);
    }
}