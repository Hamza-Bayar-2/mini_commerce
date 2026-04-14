using LogService.Application.Common.Models;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Queries.GetInfoLogs;

public record GetInfoLogsQuery : IRequest<Result<IEnumerable<Log>>>;
