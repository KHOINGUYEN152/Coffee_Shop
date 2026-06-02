using System.Collections.Generic;

namespace Coffee_shop.Models;

public class CheckoutPageViewModel
{
    public CheckoutViewModel Billing { get; set; } = new CheckoutViewModel();

    public List<CartViewItem> Items { get; set; } = new List<CartViewItem>();

    public decimal GrandTotal { get; set; }
}
