using Coffee_shop.Data;
using Coffee_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Coffee_shop.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ContactController> _logger;

    public ContactController(ApplicationDbContext db, ILogger<ContactController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new Contact());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(Contact model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.UtcNow;
            _db.Contacts.Add(model);
            _db.SaveChanges();

            TempData["ContactSuccess"] = "Cảm ơn! Tin nhắn của bạn đã được gửi.";
            _logger?.LogInformation("Contact submitted: {Email}", model.Email);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to save contact");
            TempData["ContactError"] = "Đã có lỗi xảy ra khi gửi. Vui lòng thử lại sau.";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Confirmation()
    {
        return View();
    }
}
