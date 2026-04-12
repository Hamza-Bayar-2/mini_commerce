namespace ProductService.Application.Interfaces.Services;

public interface IStatusService
{
    Task<bool> IsStatusValidAsync(short statusId, CancellationToken ct);
}