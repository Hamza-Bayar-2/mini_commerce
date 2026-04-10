using AuthService.Application.Common.Models;
using MediatR;
using System;

namespace AuthService.Application.Interfaces.Services;

public interface ICookieService
{
    Task<Result<Unit>> AppendCookies(string jwt, string refreshToken);
    Task<Result<Unit>> DeleteCookies();
}