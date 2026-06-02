using Coffee_shop.Models;

namespace Coffee_shop.Repositories;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();

    Product? GetById(int id);

    IEnumerable<Product> GetTopSelling(int count);
}