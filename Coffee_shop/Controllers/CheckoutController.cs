using System.Text.Json;
using Coffee_shop.Data;
using Coffee_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coffee_shop.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private const string SessionCartKey = "Cart";
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public CheckoutController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var ids = GetCartIds();
        if (!ids.Any()) return RedirectToAction("Empty", "Cart");

        var items = ids.GroupBy(i => i)
            .Select(g => new CartViewItem
            {
                Product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == g.Key),
                Quantity = g.Count()
            })
            .Where(ci => ci.Product != null)
            .ToList();

        var model = new CheckoutPageViewModel
        {
            Items = items,
            GrandTotal = items.Sum(i => i.Total)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CheckoutViewModel billing)
    {
        if (!ModelState.IsValid)
        {
            // Rebuild page model with cart items
            var ids = GetCartIds();
            var items = ids.GroupBy(i => i)
                .Select(g => new CartViewItem
                {
                    Product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == g.Key),
                    Quantity = g.Count()
                })
                .Where(ci => ci.Product != null)
                .ToList();

            var vm = new CheckoutPageViewModel { Billing = billing, Items = items, GrandTotal = items.Sum(i => i.Total) };
            return View(vm);
        }

        var cartIds = GetCartIds();
        if (!cartIds.Any()) return RedirectToAction("Empty", "Cart");

        var grouped = cartIds.GroupBy(i => i).ToList();

        var currentUserId = _userManager.GetUserId(User);

        var order = new Order
        {
            OrderNumber = DateTime.UtcNow.Ticks.ToString(),
            UserId = string.IsNullOrWhiteSpace(currentUserId) ? null : int.Parse(currentUserId),
            CreatedAt = DateTime.UtcNow,
            Status = "Pending",
            ShippingAddress = billing.ShippingAddress,
            PhoneNumber = billing.PhoneNumber,
            Notes = null,
            Subtotal = 0m,
            ShippingFee = 0m,
            Total = 0m
        };

        foreach (var g in grouped)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == g.Key);
            if (product == null) continue;

            var qty = g.Count();
            var unit = product.Price;

            var oi = new OrderItem
            {
                ProductId = product.Id,
                Quantity = qty,
                UnitPrice = unit
            };
            order.Items.Add(oi);

            order.Subtotal += oi.LineTotal;

            // update sold count
            product.SoldCount += qty;
        }

        // simple shipping fee: 0
        order.ShippingFee = 0m;
        order.Total = order.Subtotal + order.ShippingFee;

        _context.Orders.Add(order);
        _context.SaveChanges();

        var payment = new Payment
        {
            OrderId = order.Id,
            Method = billing.PaymentMethod ?? "Cash",
            Status = "Pending",
            Amount = order.Total
        };

        _context.Payments.Add(payment);
        _context.SaveChanges();

        // clear session cart
        HttpContext.Session.Remove(SessionCartKey);

        return RedirectToAction("Confirmation", new { id = order.Id });
    }

    public IActionResult Confirmation(int id)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .FirstOrDefault(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }

    private List<int> GetCartIds()
    {
        var json = HttpContext.Session.GetString(SessionCartKey);
        return string.IsNullOrEmpty(json)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }
}