using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;

namespace ProductService.Application.Mappings;

public static class ProductMappings
{
    public static ProductResponseDto MapToDto(Product product)
    {
        var dto = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            StatusName = GetStatusName(product.StatusId),
            Stock = product.Stock,
        };

        return dto;
    }

    private static string GetStatusName(int statusId)
    {
        return Enum.GetName(typeof(ProductStatuses), statusId) ?? "Unknown";
    }
}