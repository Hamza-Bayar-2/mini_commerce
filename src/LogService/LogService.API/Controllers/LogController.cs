using LogService.Application.Features.Logs.Queries.GetAllLogs;
using LogService.Application.Features.Queries.GetInfoLogs;
using LogService.Application.Features.Queries.GetErrorLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogController(IMediator _mediator)
    {
        this._mediator = _mediator;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllLogsQuery(), ct);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);


        return Ok(result.Data);
    }

    [HttpGet("logs/info")]
    public async Task<IActionResult> GetInfoLogs(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetInfoLogsQuery(), ct);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return Ok(result.Data);
    }

    [HttpGet("logs/error")]
    public async Task<IActionResult> GetErrorLogs(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetErrorLogsQuery(), ct);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return Ok(result.Data);
    }
}
