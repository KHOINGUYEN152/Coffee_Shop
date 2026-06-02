using System.ComponentModel.DataAnnotations;

namespace Coffee_shop.Models;

public class CheckoutViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Shipping address")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "Cash";
}
