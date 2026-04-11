namespace ProductService.Application.DTOs;

public class ProductResponseDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int Stock { get; set; }
    public required string StatusName { get; set; }
}