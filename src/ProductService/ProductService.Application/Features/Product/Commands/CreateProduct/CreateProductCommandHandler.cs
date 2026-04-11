using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductRepository _productRepo;
    private readonly IProductStatusRepository _statusRepo;

    public CreateProductCommandHandler(
    IProductRepository productRepo,
    IProductStatusRepository statusRepo)
    {
        _productRepo = productRepo;
        _statusRepo = statusRepo;
    }

    public async Task<Result<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Statü kontrolü
        var status = await _statusRepo.GetByIdAsync(request.StatusId, cancellationToken);
        if (status == null)
            return Result<ProductResponseDto>.Failure("Specified product status not found.");


        // 2. Entity oluşturma
        var product = new Domain.Entities.Product
        {
            Name = request.Name,
            Description = request.Description,
            Stock = request.Stock,
            StatusId = request.StatusId,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Repository üzerinden ekleme
        await _productRepo.AddAsync(product, cancellationToken);

        var responseDto = new ProductResponseDto
        {
            Name = product.Name,
            Description = product.Description,
            Stock = product.Stock,
            StatusName = status.Name
        };

        return Result<ProductResponseDto>.Success(responseDto);
    }
}
