namespace ProductService.Application.DTOs;

public class ProductResponseDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Stock { get; set; }
    public string? StatusName { get; set; }
}