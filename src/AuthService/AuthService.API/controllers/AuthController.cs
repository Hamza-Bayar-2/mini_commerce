using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}