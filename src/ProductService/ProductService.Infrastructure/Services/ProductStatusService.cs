using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Infrastructure.Services;

public class ProductStatusService : IStatusService
{
    private readonly IProductStatusRepository _statusRepo;

    public ProductStatusService(IProductStatusRepository statusRepo)
    {
        _statusRepo = statusRepo;
    }

    public async Task<bool> IsStatusValidAsync(short statusId, CancellationToken ct)
    {
        var status = await _statusRepo.GetByIdAsync(statusId, ct);
        return status != null;
    }
}