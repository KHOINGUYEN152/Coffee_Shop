using System;
using System.Collections.Generic;

namespace Coffee_shop.Models;

public class Cart
{
    public int Id { get; set; }

    // Optional: link cart to authenticated user (references User.Id)
    public int? UserId { get; set; }

    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
