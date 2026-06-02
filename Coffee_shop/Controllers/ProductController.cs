using Coffee_shop.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Coffee_shop.Controllers;

[Route("Products")]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet("Shop")]
    public IActionResult Shop()
    {
        return View("Index", _productRepository.GetAll());
    }

    [HttpGet("Details/{id:int}")]
    public IActionResult Details(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}