using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Services;

public class CookieService : ICookieService
{
  public CookieService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
  {
    _httpContextAccessor = httpContextAccessor;
    _config = config;
  }

  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IConfiguration _config;

  public async Task<Result<Unit>> AppendCookies(string jwt, string refreshToken)
  {
    try
    {
      var expireMinutes = int.Parse(_config["JwtSettings:ExpireMinutes"] ?? "15");

      var context = _httpContextAccessor.HttpContext;
      if (context == null)
        return Result<Unit>.Failure("HttpContext could not found");

      context.Response.Cookies.Append("credential", jwt, CreateCookieOptions(TimeSpan.FromMinutes(expireMinutes)));
      context.Response.Cookies.Append("refreshToken", refreshToken, CreateCookieOptions(TimeSpan.FromDays(14)));

      return Result<Unit>.Success(Unit.Value);
    }
    catch (Exception ex)
    {
      return Result<Unit>.Failure($"Cookie append failed: {ex.Message}");
    }
  }

  public async Task<Result<Unit>> DeleteCookies()
  {
    try
    {
      var context = _httpContextAccessor.HttpContext;
      if (context == null)
        return Result<Unit>.Failure("HttpContext could not found");

      context.Response.Cookies.Delete("credential", CreateCookieOptions());
      context.Response.Cookies.Delete("refreshToken", CreateCookieOptions());

      return Result<Unit>.Success(Unit.Value);
    }
    catch (Exception ex)
    {
      return Result<Unit>.Failure($"Cookie deletion error: {ex.Message}");
    }
  }

  private CookieOptions CreateCookieOptions(TimeSpan? maxAge = null)
  {
    var options = new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      MaxAge = maxAge,
      Path = "/"
    };
    
    var cookieDomain = _config["Cookie:Domain"];
    if (!string.IsNullOrWhiteSpace(cookieDomain))
    {
      options.Domain = cookieDomain;
    }

    return options;
  }
}