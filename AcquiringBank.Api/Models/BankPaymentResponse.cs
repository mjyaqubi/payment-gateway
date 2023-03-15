
namespace AcquiringBank.Api.Models
{
    public class BankPaymentResponse
    {
        public Guid Id { get; set; }

        public Guid PaymentGatewayOrderId { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public BankPaymentStatus Status { get; set; }

        public BankPaymentDeclineReason? DeclineReason { get; set; }
    }
}
