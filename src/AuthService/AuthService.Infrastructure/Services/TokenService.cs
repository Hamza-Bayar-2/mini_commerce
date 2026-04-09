using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;

namespace AuthService.Infrastructure.Services;

public class TokenService(IRefreshTokenRepository refreshTokenRepo) : ITokenService
{
  private readonly IRefreshTokenRepository _refreshTokenRepo = refreshTokenRepo;

  public async Task<Result<string>> GenerateAccessTokenAsync(User user, IList<string> roles, CancellationToken ct = default)
  {
    throw new NotImplementedException();
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
      var oldToken = await _refreshTokenRepo.GetById(oldTokenId.Value, ct);
      if (oldToken != null)
      {
        oldToken.IsRevoked = true;
        oldToken.ReplacedBy = newRefreshTokenEntity.Id;
        await _refreshTokenRepo.Update(oldToken);
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

  public Task<Result<UserRefreshToken>> ValidateRefreshTokenAsync(Guid userId, string refreshToken, DateTime now, CancellationToken ct = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<bool>> VerifyToken(string token, string hash)
  {
    throw new NotImplementedException();
  }
}