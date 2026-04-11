using AuthService.Application.Interfaces;

namespace AuthService.Application.Common.Models;

public record Result<T>(T? Data, bool IsSuccess, string? ErrorMessage) : IResult
{
    public static Result<T> Success(T data) => new(data, true, null);
    public static Result<T> Failure(string errorMessage) => new(default, false, errorMessage);
}