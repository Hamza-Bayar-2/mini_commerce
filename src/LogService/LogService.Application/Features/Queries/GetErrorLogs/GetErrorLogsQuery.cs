using LogService.Application.Common.Models;
using LogService.Domain.Entities;
using MediatR;

namespace LogService.Application.Features.Queries.GetErrorLogs;

public record GetErrorLogsQuery : IRequest<Result<IEnumerable<Log>>>;
