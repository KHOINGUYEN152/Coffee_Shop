using System.Text.Json;
using Coffee_shop.Models;
using Coffee_shop.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Coffee_shop.Controllers;

public class CartController : Controller
{
    private const string SessionCartKey = "Cart";
    private readonly IProductRepository _productRepository;

    public CartController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IActionResult Index()
    {
        var ids = GetCartIds();

        if (!ids.Any())
        {
            return View("Empty");
        }

        var grouped = ids.GroupBy(i => i)
            .Select(g => new CartViewItem
            {
                Product = _productRepository.GetById(g.Key)!,
                Quantity = g.Count()
            })
            .Where(ci => ci.Product != null)
            .ToList();

        var model = new CartViewModel
        {
            Items = grouped
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Add(int id)
    {
        var ids = GetCartIds();

        ids.Add(id);
        HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(ids));

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AddAjax(int id)
    {
        var ids = GetCartIds();

        ids.Add(id);
        HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(ids));

        return Json(new
        {
            success = true,
            cartCount = ids.Count
        });
    }

    [HttpGet]
    public IActionResult Remove(int id)
    {
        var ids = GetCartIds();

        // Xóa tất cả sản phẩm có id này khỏi giỏ hàng
        ids.RemoveAll(i => i == id);
        HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(ids));

        return RedirectToAction("Index");
    }

    private List<int> GetCartIds()
    {
        var json = HttpContext.Session.GetString(SessionCartKey);
        return string.IsNullOrEmpty(json)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }
}