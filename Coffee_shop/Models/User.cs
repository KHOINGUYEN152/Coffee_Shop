using Microsoft.AspNetCore.Identity;

namespace Coffee_shop.Models;

public class User : IdentityUser<int>
{
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
