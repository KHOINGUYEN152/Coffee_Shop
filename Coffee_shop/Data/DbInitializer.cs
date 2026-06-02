using Coffee_shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coffee_shop.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Ensure legacy RoleId column on AspNetUsers allows NULL so Identity can create users
        try
        {
            var sql = "ALTER TABLE [AspNetUsers] ALTER COLUMN [RoleId] INT NULL;";
            await context.Database.ExecuteSqlRawAsync(sql);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: could not alter AspNetUsers.RoleId to NULL: " + ex.Message);
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role { Name = "Admin" });
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new Role { Name = "Customer" });
        }
    }
}