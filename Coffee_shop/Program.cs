using Coffee_shop.Data;
using Coffee_shop.Models;
using Coffee_shop.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(1);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (string.Equals(context.Request.Path, "/shop.html", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Products/Shop", permanent: false);
        return;
    }

    if (string.Equals(context.Request.Path, "/contact.html", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Contact/Index", permanent: false);
        return;
    }

    if (string.Equals(context.Request.Path, "/cart.html", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(context.Request.Path, "/shopping-cart.html", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Cart/Index", permanent: false);
        return;
    }

    await next();
});

await DbInitializer.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// Ensure root URL "/" goes to Home/Index explicitly (helps when attribute routing is used)
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
