namespace ProductService.Domain.Entities;

public class Product
{
    public Product()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public short StatusId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ProductStatus Status { get; set; } = null!;
}
