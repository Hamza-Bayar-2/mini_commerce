using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Services;

public class TokenService : ITokenService
{
  private readonly IRefreshTokenRepository _refreshTokenRepo;
  private readonly IConfiguration _config;

  public TokenService(IRefreshTokenRepository refreshTokenRepo, IConfiguration config)
  {
    _refreshTokenRepo = refreshTokenRepo;
    _config = config;
  }

  public async Task<Result<string>> GenerateAccessTokenAsync(User userObj, IList<string> roles, CancellationToken ct = default)
  {
    if (userObj is not User user)
    {
      return Result<string>.Failure("User object is not of type User: " + nameof(userObj));
    }

    // TODO buraya bir validator ekle
    var jwtKey = _config["Jwt:Key"] ?? "ThisIsASecretKeyForDevOnly!ChangeIt";
    var jwtIssuer = _config["Jwt:Issuer"] ?? "fxaret.local";
    var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "15");

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);

    var nowUtc = DateTime.UtcNow;
    var expiresUtc = nowUtc.AddMinutes(expireMinutes);

    var claims = new List<Claim>
      {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(nowUtc).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
      };

    foreach (var role in roles)
    {
      claims.Add(new(ClaimTypes.Role, role));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Issuer = jwtIssuer,
      NotBefore = nowUtc,
      Expires = expiresUtc,
      SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Result<string>.Success(tokenHandler.WriteToken(token));
  }

  public async Task<Result<(UserRefreshToken Entity, string UnhashedToken)>> GenerateRefreshTokenAsync(Guid userId, Guid? oldTokenId, DateTime now, CancellationToken ct = default)
  {

    var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    var result = await HashToken(newRefreshToken);
    if (!result.IsSuccess)
      return Result<(UserRefreshToken, string)>.Failure(result.ErrorMessage!);

    var newRefreshTokenEntity = new UserRefreshToken
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      TokenHash = result.Data!,
      ExpiresAt = now.AddDays(14),
      IsRevoked = false,
      CreatedAt = now,
    };

    if (oldTokenId != null)
    {
      // Eski token ı bul ve iptal et/güncelle
      var oldToken = await _refreshTokenRepo.GetByIdAsync(oldTokenId.Value, ct);
      if (oldToken != null)
      {
        oldToken.IsRevoked = true;
        oldToken.ReplacedBy = newRefreshTokenEntity.Id;
        oldToken.LastUsedAt = now;
        await _refreshTokenRepo.UpdateAsync(oldToken);
      }
    }

    await _refreshTokenRepo.AddAsync(newRefreshTokenEntity, ct);

    return Result<(UserRefreshToken, string)>.Success((newRefreshTokenEntity, newRefreshToken));
  }

  public Task<Result<string>> HashToken(string token)
  {
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    return Task.FromResult(Result<string>.Success(Convert.ToBase64String(bytes)));
  }

  public async Task<Result<UserRefreshToken>> ValidateRefreshTokenAsync(string tokenString, DateTime now, CancellationToken ct = default)
  {
    var hashTokenResult = await HashToken(tokenString);

    if (!hashTokenResult.IsSuccess)
      return Result<UserRefreshToken>.Failure(hashTokenResult.ErrorMessage!);

    var existingRefreshToken = await _refreshTokenRepo.GetByTokenAsync(hashTokenResult.Data!, ct);

    if (existingRefreshToken == null)
      return Result<UserRefreshToken>.Failure("Refresh Tokne does not exist");

    if (existingRefreshToken.IsRevoked == true || existingRefreshToken.ExpiresAt < now)
      return Result<UserRefreshToken>.Failure("Refresh token süresi dolmuş veya iptal edilmiş.");

    return Result<UserRefreshToken>.Success(existingRefreshToken);
  }

  public Task<Result<bool>> VerifyToken(string token, string hash)
  {
    throw new NotImplementedException();
  }
}