using System;

namespace Coffee_shop.Models;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public string Method { get; set; } = "Cash";

    public string Status { get; set; } = "Pending";

    public decimal Amount { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }
}
