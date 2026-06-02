using Coffee_shop.Data;
using Coffee_shop.Models;
using Microsoft.EntityFrameworkCore;

namespace Coffee_shop.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products
            .Include(product => product.Category)
            .OrderBy(product => product.Name)
            .ToList();
    }

    public Product? GetById(int id)
    {
        return _context.Products
            .Include(product => product.Category)
            .FirstOrDefault(product => product.Id == id);
    }

    public IEnumerable<Product> GetTopSelling(int count)
    {
        return _context.Products
            .Include(product => product.Category)
            .OrderByDescending(product => product.SoldCount)
            .ThenBy(product => product.Name)
            .Take(count)
            .ToList();
    }
}