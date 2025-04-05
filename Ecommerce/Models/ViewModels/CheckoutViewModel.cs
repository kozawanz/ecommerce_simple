using System.ComponentModel.DataAnnotations;

namespace eCommerce.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public IEnumerable<ShoppingCart> CartItems { get; set; } = new List<ShoppingCart>();
    }
}

