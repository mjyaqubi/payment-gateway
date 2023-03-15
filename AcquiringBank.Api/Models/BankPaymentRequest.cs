using System.ComponentModel.DataAnnotations;

namespace AcquiringBank.Api.Models
{
    public class BankPaymentRequest
    {
        [Required(ErrorMessage = "Payment gateway order id is required")]
        public Guid PaymentGatewayOrderId { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number is not valid")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Card number is not valid")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry month is required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Expiry month is not valid")]
        [RegularExpression("^(0?[1-9]|1[012])$", ErrorMessage = "Expiry month is not valid")]
        public string ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Expiry year is required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Expiry year is not valid")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Expiry year is not valid")]
        public string ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV is not valid")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "CVV is not valid")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency is not valid")]
        [RegularExpression("^[A-Z]*$", ErrorMessage = "Currency is not valid")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 9999999999999999.99, ErrorMessage = "Amount is not valid")]
        public decimal Amount { get; set; }
    }
}

