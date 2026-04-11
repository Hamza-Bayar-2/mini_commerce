using AuthService.Application.Common.Models;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Services;

public interface ITokenService
{
    Task<Result<string>> GenerateAccessTokenAsync(User user, IList<string> roles, CancellationToken ct);
    Task<Result<(UserRefreshToken Entity, string UnhashedToken)>> GenerateRefreshTokenAsync(Guid userId, Guid? oldTokenId, DateTime now, CancellationToken ct);
    /// <summary>
    /// Returns the token entity if it is not revoked nor expired
    /// </summary>
    /// <param name="tokenString"></param>
    /// <param name="now"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Result<UserRefreshToken>> ValidateRefreshTokenAsync(string tokenString, DateTime now, CancellationToken ct);
    /// <summary>
    /// Convert the token string to a hashed token string to store it safely.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Result<string>> HashToken(string token);
    /// <summary>
    /// Verify if the token string and the hashed token string are the same
    /// </summary>
    /// <param name="token"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    Task<Result<bool>> VerifyToken(string token, string hash);

}
