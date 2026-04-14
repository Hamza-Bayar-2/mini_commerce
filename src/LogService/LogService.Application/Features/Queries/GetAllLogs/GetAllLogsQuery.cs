using LogService.Application.Common.Models;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Logs.Queries.GetAllLogs;

public record GetAllLogsQuery : IRequest<Result<IEnumerable<Log>>>;