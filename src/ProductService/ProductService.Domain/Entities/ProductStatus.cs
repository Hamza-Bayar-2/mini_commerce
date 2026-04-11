namespace ProductService.Domain.Entities;

public class ProductStatus
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
