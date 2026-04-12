using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.WebUtilities;

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
        if (userObj == null)
            return Result<string>.Failure("User object cannot be null.");

        // Yapılandırma değerlerini güvenli oku
        var jwtKey = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
        var jwtIssuer = _config["JwtSettings:Issuer"] ?? "mini_commerce.auth";
        var jwtAudience = _config["JwtSettings:Audience"] ?? "mini_commerce.auth";
        var expireMinutes = int.Parse(_config["JwtSettings:ExpireMinutes"] ?? "15");

        var key = Encoding.UTF8.GetBytes(jwtKey);
        var nowUtc = DateTime.UtcNow;

        var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, userObj.Id.ToString()),
      new(JwtRegisteredClaimNames.Email, userObj.Email),
      new(ClaimTypes.NameIdentifier, userObj.Id.ToString()),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(nowUtc).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims, "Jwt"),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            NotBefore = nowUtc,
            Expires = nowUtc.AddMinutes(expireMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.CreateToken(tokenDescriptor);

        return Result<string>.Success(tokenHandler.WriteToken(accessToken));
    }

    public async Task<Result<(UserRefreshToken Entity, string UnhashedToken)>> GenerateRefreshTokenAsync(Guid userId, Guid? oldTokenId, DateTime now, CancellationToken ct = default)
    {

        var bytes = RandomNumberGenerator.GetBytes(64);
        var unhashedToken = WebEncoders.Base64UrlEncode(bytes);

        var hashedTokenResult = await HashToken(unhashedToken);
        if (!hashedTokenResult.IsSuccess)
            return Result<(UserRefreshToken, string)>.Failure(hashedTokenResult.ErrorMessage!);

        var newRefreshTokenEntity = new UserRefreshToken
        {
            UserId = userId,
            TokenHash = hashedTokenResult.Data!,
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

        return Result<(UserRefreshToken, string)>.Success((newRefreshTokenEntity, unhashedToken));
    }

    public Task<Result<string>> HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToBase64String(bytes);
        return Task.FromResult(Result<string>.Success(tokenHash));
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

    public async Task<Result<bool>> VerifyToken(string token, string hash)
    {
        var hashResult = await HashToken(token);
        if (!hashResult.IsSuccess)
            return Result<bool>.Failure(hashResult.ErrorMessage!);

        return Result<bool>.Success(hashResult.Data == hash);
    }
}