using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.Logout;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Features.Auth.Queries.GetUserInfo;
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
        var registerResult = await _mediator.Send(command);
        if (!registerResult.IsSuccess)
            return BadRequest(new { Error = registerResult.ErrorMessage });

        return Ok(registerResult.Data);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return NoContent();
    }

    [HttpGet("user-info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _mediator.Send(new GetUserInfoQuery());
        if (!result.IsSuccess)
            return NotFound(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }
}