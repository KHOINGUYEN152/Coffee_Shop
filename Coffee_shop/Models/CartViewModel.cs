using System.Collections.Generic;
using System.Linq;

namespace Coffee_shop.Models;

public class CartViewItem
{
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    public decimal Total => (Product?.Price ?? 0) * Quantity;
}

public class CartViewModel
{
    public List<CartViewItem> Items { get; set; } = new List<CartViewItem>();

    public decimal GrandTotal => Items.Sum(i => i.Total);
}
