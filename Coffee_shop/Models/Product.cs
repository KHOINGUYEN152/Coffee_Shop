namespace Coffee_shop.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    // Số lượng đã bán (dùng để tính sản phẩm bán chạy nhất)
    public int SoldCount { get; set; } = 0;

    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}